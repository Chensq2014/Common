// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Protos/lesson.proto
// </auto-generated>
#pragma warning disable 0414, 1591
#region Designer generated code

using grpc = global::Grpc.Core;

namespace Parakeet.NetCore.GrpcLessonServer {
  /// <summary>
  /// The Lesson service definition.
  /// </summary>
  public static partial class Lesson
  {
    static readonly string __ServiceName = "Lesson.Lesson";

    static readonly grpc::Marshaller<global::Parakeet.NetCore.GrpcLessonServer.LessonRequest> __Marshaller_Lesson_LessonRequest = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Parakeet.NetCore.GrpcLessonServer.LessonRequest.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::Parakeet.NetCore.GrpcLessonServer.LessonReply> __Marshaller_Lesson_LessonReply = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Parakeet.NetCore.GrpcLessonServer.LessonReply.Parser.ParseFrom);

    static readonly grpc::Method<global::Parakeet.NetCore.GrpcLessonServer.LessonRequest, global::Parakeet.NetCore.GrpcLessonServer.LessonReply> __Method_FindLesson = new grpc::Method<global::Parakeet.NetCore.GrpcLessonServer.LessonRequest, global::Parakeet.NetCore.GrpcLessonServer.LessonReply>(
        grpc::MethodType.Unary,
        __ServiceName,
        "FindLesson",
        __Marshaller_Lesson_LessonRequest,
        __Marshaller_Lesson_LessonReply);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Parakeet.NetCore.GrpcLessonServer.LessonReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of Lesson</summary>
    [grpc::BindServiceMethod(typeof(Lesson), "BindService")]
    public abstract partial class LessonBase
    {
      /// <summary>
      /// Sends a greeting
      /// </summary>
      /// <param name="request">The request received from the client.</param>
      /// <param name="context">The context of the server-side call handler being invoked.</param>
      /// <returns>The response to send back to the client (wrapped by a task).</returns>
      public virtual global::System.Threading.Tasks.Task<global::Parakeet.NetCore.GrpcLessonServer.LessonReply> FindLesson(global::Parakeet.NetCore.GrpcLessonServer.LessonRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(LessonBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_FindLesson, serviceImpl.FindLesson).Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the  service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static void BindService(grpc::ServiceBinderBase serviceBinder, LessonBase serviceImpl)
    {
      serviceBinder.AddMethod(__Method_FindLesson, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::Parakeet.NetCore.GrpcLessonServer.LessonRequest, global::Parakeet.NetCore.GrpcLessonServer.LessonReply>(serviceImpl.FindLesson));
    }

  }
}
#endregion
