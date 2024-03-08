using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Common.RabbitMQModule.Channels.Abstractions;
using Common.RabbitMQModule.Client;
using Common.RabbitMQModule.Core;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Common.Storage;
using Serilog;

namespace Common.RabbitMQModule.Consumers
{
    /// <summary>
    /// 运行的消费者(字典集(不重复)) 由上端new出来
    /// </summary>
    public class ConsumerRunner
    {
        /// <summary>
        /// 日志
        /// </summary>
        public ILogger<ConsumerRunner> Logger { get; }

        /// <summary>
        /// 客户端 
        /// </summary>
        public IRabbitMQClient Client { get; }

        /// <summary>
        /// RabbitMQ.Client.Events BasicDeliverEventArgs 多生产者单消费者管道消费事件
        /// </summary>
        private readonly IMpscChannel<BasicDeliverEventArgs> _mpscChannel;

        /// <summary>
        /// 通道代理扩展类
        /// </summary>
        public ModelWrapper ModelWrapper { get; set; }

        /// <summary>
        /// RabbitMQ.Client.Events.EventingBasicConsumer 这才是真正的消费者 (RabbitMQ.Client.dll)
        /// </summary>
        public EventingBasicConsumer BasicConsumer { get; set; }

        /// <summary>
        /// 扩展的RabbitMQ消费者
        /// </summary>
        public RabbitMQConsumer Consumer { get; }

        /// <summary>
        /// 队列信息
        /// </summary>
        public QueueInfo QueueInfo { get; }

        /// <summary>
        /// 是否不可用
        /// </summary>
        public bool IsUnAvailable => BasicConsumer?.IsRunning == false || ModelWrapper.Channel.IsClosed;

        private bool _isFirst = true;

        public ConcurrentQueue<ulong> BatchUnackeds { get; private set; } = new ConcurrentQueue<ulong>();

        public ConcurrentQueue<ulong> Unackeds { get; private set; } = new ConcurrentQueue<ulong>();

        /// <summary>
        /// 消费失败数据确认检查
        /// </summary>
        protected System.Timers.Timer UnackedTimer = new System.Timers.Timer();

        public ConsumerRunner(IRabbitMQClient client, IServiceProvider provider, RabbitMQConsumer consumer, QueueInfo queue)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(ConsumerRunner)} 被构造 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            Client = client;
            Logger = provider.GetService<ILogger<ConsumerRunner>>();
            _mpscChannel = provider.GetService<IMpscChannel<BasicDeliverEventArgs>>();
            _mpscChannel.BindConsumerFunc(BatchExecute);
            Consumer = consumer;
            QueueInfo = queue;
            UnackedTimer.Interval = 1000;
            UnackedTimer.Elapsed += UnackedTimer_Elapsed;
            UnackedTimer.Start();
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(ConsumerRunner)} 构造完毕 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        }

        /// <summary>
        /// 消费失败数据检查绑定事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnackedTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            while (BatchUnackeds.TryDequeue(out var result))
            {
                ModelWrapper.Channel.BasicAck(result, true);
            }

            while (Unackeds.TryDequeue(out var result))
            {
                ModelWrapper.Channel.BasicAck(result, false);
            }
        }


        /// <summary>
        /// 运行消费者 消费消息队列数据
        /// </summary>
        /// <returns></returns>
        public Task Run()
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(ConsumerRunner)} Run 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            ModelWrapper = Client.PullModel();
            _mpscChannel.Config(ModelWrapper.ConnectionWrapper.Options.ConsumerMaxBatchSize, ModelWrapper.ConnectionWrapper.Options.ConsumerMaxMillisecondsInterval);

            #region 仅首次运行消费者时，声明交换机和队列，因重复声明会报错

            if (_isFirst)
            {
                _isFirst = false;
                ModelWrapper.Channel.ExchangeDeclare
                (
                    exchange: Consumer.EventBusExchange,
                    type: Consumer.ExchangeType,
                    durable: true,
                    autoDelete: false,
                    arguments: null
                 );
                ModelWrapper.Channel.QueueDeclare
                (
                    queue: QueueInfo.Queue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: QueueInfo.Arguments
                 );
                Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(ConsumerRunner)} Run 首次运行时候声明交换机_队列{Consumer.EventBusExchange}_{QueueInfo.Queue} 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            }

            #endregion

            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(ConsumerRunner)} Run 逻辑:通道代理绑定队列 ModelWrapper.Channel.QueueBind，队列分流批量处理配置ModelWrapper.Channel.BasicQos，绑定消费者：ModelWrapper.Channel.BasicConsume，接收数据事件绑定逻辑，注意：BasicConsumer才是RabbitMQ真正的消费者，当它接收到数据的触发Received事件时，会级联触发_mpscChannel(多生产单消费者)的写入数据的事件 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            ModelWrapper.Channel.QueueBind(QueueInfo.Queue, Consumer.EventBusExchange, QueueInfo.RoutingKey);
            ModelWrapper.Channel.BasicQos(0, ModelWrapper.ConnectionWrapper.Options.ConsumerMaxBatchSize, false);
            BasicConsumer = new EventingBasicConsumer(ModelWrapper.Channel);
            BasicConsumer.Received += async (sender, eventArgs) => await _mpscChannel.WriteAsync(eventArgs);
            ModelWrapper.Channel.BasicConsume(QueueInfo.Queue, Consumer.Config.AutoAck, BasicConsumer);

            return Task.CompletedTask;
        }

        /// <summary>
        /// 健康检查，如果消息队列离线后重启，那么将尝试重新开启启动消息监听
        /// </summary>
        /// <returns></returns>
        public Task HeathCheck()
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(ConsumerRunner)} HeathCheck 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            if (IsUnAvailable)
            {
                Close();
                return Run();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// 数据连接，通道代理，生产者，消费者，发送处理数据委托，事件参数等等，都被封装了(包了一层)。
        /// 包一层的好处不用多说，肯定是为了更好的扩展，添加缓存提高性能等，只是被封装了多个层次，而且相互之间调来调去
        /// 看起来眼花缭乱，宝宝不耐烦，没办法，封装一定有它的道理，一点一点看吧...
        /// 比如这个消费者委托就是扩展了好几十个自定义类和方法【有点夸张但是还可以精简】，
        /// 咱们就来梳理一下这个消费者委托执行流程：
        ///
        /// 首先，从消费者委托的源头看起： ConsumerRunner Run方法中:
        /// 正真的RabbitMQ官方的的消费者 BasicConsumer 以及它的接收数据事件BasicConsumer.Received 通道代理ModelWrapper.Channel
        /// 消费者最关键的三句话：
        ///    BasicConsumer = new EventingBasicConsumer(ModelWrapper.Channel);//消费者是被new 出来的
        ///    BasicConsumer.Received += async(sender, eventArgs) => await _mpscChannel.WriteAsync(eventArgs);
        ///    ModelWrapper.Channel.BasicConsume(QueueInfo.Queue, Consumer.Config.AutoAck, BasicConsumer);
        ///
        ///  消费者真正的Received事件-->调用了_mpscChannel.WriteAsync【_mpscChannel：多生产者单消费者(基于通道代理channel)】
        ///   -->判断_consumerFunc委托是否赋值，赋值之后执行ActiveAutoConsumer
        ///   -->ActiveAutoConsumer开始 线程池排队处理--调用ActiveConsumer(单线程模式，优化性能，数据量太大就会写入_buffer缓冲区)，
        ///   -->判断_buffer中是否还有可以处理的缓冲数据，经过一系列折腾之后，最终手动调用的消费者常规批量消费方法ManualConsume 
        ///   -->ManualConsume 内部：从缓冲区_buffer当中把数据取出来(可以控制一次性取多少数据，取数时间等)，递归子通道ManualConsume
        ///   -->取到数据序列化后得到datalist
        ///   -->执行 await _consumerFunc(dataList) 【这个委托才被真正invoke，即BatchExecute(dataList)被调用】
        ///   -->接着再看 BatchExecute 这个委托怎么跟_consumerFunc关联起来的，请继续....
        ///
        /// 其次：再看 BatchExecute 这个委托怎么跟_consumerFunc关联起来的
        /// BatchExecute(消费者处理数据的自定义委托) 它在ConsumerRunner的构造函数中
        /// 被依赖注入的_mpscChannel(多生单消类)直接绑定消费者委托(_mpscChannel.BindConsumerFunc(BatchExecute)
        /// MpscChannel构造函数中直接绑定真正消费者消费数据的委托_consumerFunc=BatchExecute)
        /// 所以，当消费者消费数据时_consumerFunc(dataList)，相当于此委托【BatchExecute(dataList)】被调用！！
        ///
        /// 最后：再看
        /// BatchExecute委托内部逻辑：
        ///     如果单条数据又会去执行Process委托方法
        ///         Process委托内部会调用自定义Consumer.Notice委托方法
        ///             Notice内部会调用EventHandlers委托事件(执行扩展的单条数据自定义消费委托事件)
        ///     多条数据就会 调用自定义批量Consumer.Notice方法
        ///         批量Notice内部会调用BatchEventHandlers委托事件(执行自定义批量消费的委托事件)
        /// 所以，最终委托都会一直关联到 Consumer基类的 EventHandlers 与BatchEventHandlers 事件委托。
        /// 所以，所有继承了Consumer子类，只要重写了EventHandlers与BatchEventHandlers委托 就能从消息队列里面获取数据进行数据消费。
        /// 哦，这下明白了，这下有没有很清晰了呢，全都是接口，根据业务封装扩展类和方法，灵活运用而已,
        /// 只是NetCore里面事件，委托无处不再，会运用即可。
        /// </summary>
        /// <param name="list">BasicDeliverEventArgs集合</param>
        /// <returns></returns>
        private async Task BatchExecute(List<BasicDeliverEventArgs> list)
        {
            if (list.Count == 1)
            {
                //Process委托内部会调用Consumer.Notice方法-->Notice内部会调用EventHandlers 执行自定义扩展的委托
                await Process(list.First());
            }
            else
            {
                var maxDeliveryTag = list.Max(o => o.DeliveryTag);
                try
                {
                    //这里也是手动调用的批量 Notice方法 封装得深哟。
                    await Consumer.Notice(list.Select(o => (Encoding.UTF8.GetString(o.Body), o.DeliveryTag)).ToList());//5.2.0
                    //await Consumer.Notice(list.Select(o => (Encoding.UTF8.GetString(o.Body.Span), o.DeliveryTag)).ToList());//6.2.1  5.2.0版本以上，消息队列大小被默认限制(约2MB)，消息内容超出默认限制大小，消费时就会导致序列化溢出
                    if (!Consumer.Config.AutoAck)
                    {
                        await HeathCheck();
                        //消息手动批量确认
                        ModelWrapper.Channel.BasicAck(maxDeliveryTag, true);
                    }
                }
                catch (Exception exception)
                {
                    if (exception is OperationInterruptedException operationInterruptedException)
                    {
                        if (operationInterruptedException.ShutdownReason.ReplyCode == 406)
                        {
                            BatchUnackeds.Enqueue(maxDeliveryTag);
                        }
                        return;
                    }
                    Logger.LogInformation($"批量处理队列数据异常 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                    if (Consumer.BathHandlePartialFailed)
                    {
                        Logger.LogInformation($"批量处理队列数据异常，部分处理成功 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                        if (Consumer.AckedTags.Any())
                        {
                            ModelWrapper.Channel.BasicAck(Consumer.AckedTags.Max(), true);
                        }
                        Logger.LogInformation($"批量处理队列数据异常，改为单个处理 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                        foreach (var eventArgse in list.Where(x => !Consumer.AckedTags.Contains(x.DeliveryTag)))
                        {
                            await Process(eventArgse);
                        }
                        Consumer.AckedTags.Clear();
                    }
                    else
                    {
                        Logger.LogInformation($"批量处理队列数据异常，改为单个处理 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                        foreach (var eventArgse in list)
                        {
                            await Process(eventArgse);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 单条数据处理程序(批量失败最终也会调用这个处理程序)
        /// </summary>
        /// <param name="eventArgs">BasicDeliverEventArgs</param>
        /// <returns></returns>
        private async Task Process(BasicDeliverEventArgs eventArgs)
        {
            try
            {
                //这个地方调用的Notice方法，真是封装得深啊！！！

                await Consumer.Notice(Encoding.UTF8.GetString(eventArgs.Body), eventArgs.DeliveryTag);//5.2.0
                //await Consumer.Notice(Encoding.UTF8.GetString(eventArgs.Body.Span), eventArgs.DeliveryTag);//6.2.1
                if (!Consumer.Config.AutoAck)
                {
                    await HeathCheck();
                    //消息手动单条确认 ack:acknowledg 应答
                    ModelWrapper.Channel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
                }
            }
            catch (Exception exception)
            {
                if (exception is OperationInterruptedException operationInterruptedException)
                {
                    if (operationInterruptedException.ShutdownReason.ReplyCode == 406)
                    {
                        Unackeds.Enqueue(eventArgs.DeliveryTag);
                    }
                    return;
                }
                Logger.LogError(exception.InnerException ?? exception, $"【单消息处理】异常消息队列信息=> EventBusExchange:{Consumer.EventBusExchange};Queue:{QueueInfo.Queue} 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                if (Consumer.Config.Requeue)
                {
                    await Task.Delay(1000);
                    ModelWrapper.Channel.BasicReject(deliveryTag:eventArgs.DeliveryTag, requeue:true);
                }
            }
        }


        public void Close()
        {
            ModelWrapper?.Dispose();
            UnackedTimer.Close();
        }
    }
}