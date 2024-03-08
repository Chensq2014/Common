﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Common.Enums;

namespace Common.Dtos
{
    /// <summary>
    /// 设备人员
    /// </summary>
    public class DeviceWorkerDto : DeviceWorkerBaseDto
    {
        #region 基础字段

        /// <summary>
        /// 省级Area
        /// </summary>
        [MaxLength(CommonConsts.MaxLength16)]
        [Description("省级Area")]
        public string ProvenceArea { get; set; }

        /// <summary>
        /// 身份证sha1加密
        /// </summary>
        [MaxLength(CommonConsts.MaxLength64)]
        [Description("身份证sha1加密")]
        public string IdCardEncrypt { get; set; }

        /// <summary>
        /// 所属企业ID
        /// </summary>
        [MaxLength(CommonConsts.MaxLength64)]
        [Description("所属企业ID")]
        public string CorpId { get; set; }

        /// <summary>
        /// 企业编码 CompanyNo
        /// </summary>
        [MaxLength(CommonConsts.MaxLength64)]
        [Description("企业编码 CompanyNo")]
        public string CorpCode { get; set; }

        /// <summary>
        /// 企业名称
        /// </summary>
        [MaxLength(CommonConsts.MaxLength64)]
        [Description("企业名称")]
        public string CorpName { get; set; }

        /// <summary>
        /// 工种ID
        /// </summary>
        [MaxLength(CommonConsts.MaxLength64)]
        [Description("工种ID")]
        public string WorkerTypeId { get; set; }

        /// <summary>
        /// 工种编码
        /// </summary>
        [MaxLength(CommonConsts.MaxLength64)]
        [Description("工种编码")]
        public string WorkerTypeCode { get; set; }

        /// <summary>
        /// 工种名称 工种 WorkTypeName Job
        /// </summary>
        [MaxLength(CommonConsts.MaxLength64)]
        [Description("工种名称 工种 WorkTypeName Job")]
        public string WorkerTypeName { get; set; }

        /// <summary>
        /// 岗位ID
        /// </summary>
        [MaxLength(CommonConsts.MaxLength64)]
        [Description("岗位ID")]
        public string WorkPostId { get; set; }

        /// <summary>
        /// 岗位编码
        /// </summary>
        [MaxLength(CommonConsts.MaxLength64)]
        [Description("岗位编码")]
        public string WorkPostCode { get; set; }

        /// <summary>
        /// 岗位名称 AdminPostName AdminPost
        /// </summary>
        [MaxLength(CommonConsts.MaxLength64)]
        [Description("岗位名称 AdminPostName AdminPost")]
        public string WorkPostName { get; set; }

        /// <summary>
        /// 班组ID
        /// </summary>
        [MaxLength(CommonConsts.MaxLength64)]
        [Description("班组ID")]
        public string WorkerGroupId { get; set; }

        /// <summary>
        /// 班组编码
        /// </summary>
        [MaxLength(CommonConsts.MaxLength64)]
        [Description("班组编码")]
        public string WorkerGroupCode { get; set; }

        /// <summary>
        /// 班组名称 GroupName
        /// </summary>
        [Description("班组名称 GroupName")]
        [MaxLength(CommonConsts.MaxLength64)]
        public string WorkerGroupName { get; set; }

        /// <summary>
        /// 人员状态： Entry： 在职 Exit： 离职 Locked：禁入
        /// </summary>
        [Description("人员状态： Entry： 在职 Exit： 离职 Locked：禁入")]
        [MaxLength(CommonConsts.MaxLength32)]
        public string EntryStatus { get; set; }

        /// <summary>
        /// 入职时间
        /// </summary>
        [Description("入职时间")]
        public DateTime? JoinDate { get; set; }

        /// <summary>
        /// 离职时间
        /// </summary>
        [Description("离职时间")]
        public DateTime? LeaveDate { get; set; }

        /// <summary>
        /// 考勤卡ID
        /// </summary>
        [MaxLength(CommonConsts.MaxLength64)]
        [Description("考勤卡ID")]
        public string AttendanceCardId { get; set; }

        /// <summary>
        /// 考勤卡号
        /// </summary>
        [MaxLength(CommonConsts.MaxLength64)]
        [Description("考勤卡号")]
        public string AttendanceCardNo { get; set; }

        /// <summary>
        /// 考勤方式：
        /// Face： 人脸识别
        /// Eye： 虹膜识别
        /// Finger： 指纹识别
        /// Hand： 掌纹识别
        /// IDCard： 身份证识别
        /// RnCard： 实名卡
        /// Error： 异常清退
        /// Manuel： 一键开闸
        /// ExitChannel： 应急通道
        /// QRCode： 二维码识别
        /// App： APP考勤
        /// Other： 其他方式
        /// </summary>
        [MaxLength(CommonConsts.MaxLength32)]
        [Description("考勤方式 Face： 人脸识别 Eye： 虹膜识别 Finger： 指纹识别 Hand： 掌纹识别 IDCard： 身份证识别 RnCard： 实名卡 Error： 异常清退 Manuel： 一键开闸 ExitChannel： 应急通道 QRCode： 二维码识别 App： APP考勤 Other： 其他方式")]
        public string Mode { get; set; }

        /// <summary>
        /// 考勤卡类别
        /// </summary>
        [Description("考勤卡类别")]
        [MaxLength(CommonConsts.MaxLength64)]
        public string AttendanceCardType { get; set; }

        /// <summary>
        /// 制卡时间
        /// </summary>
        [Description("制卡时间")]
        public DateTime? AttendanceCardIssueDate { get; set; }

        /// <summary>
        /// 制卡图片
        /// </summary>
        [Description("制卡图片")]
        [MaxLength(CommonConsts.MaxLength64)]
        public string AttendanceCardIssuePic { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        [Description("最后更新时间")]
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 是否班组长
        /// </summary>
        [Description("是否班组长")]
        public bool GroupLeader { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        [MaxLength(CommonConsts.MaxLength64)]
        [Description("电话号码")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 婚姻状况
        /// </summary>
        [MaxLength(CommonConsts.MaxLength32)]
        [Description("婚姻状况")]
        public string Marital { get; set; }

        /// <summary>
        /// 政治面貌 （ 0.群众 30.中共党员 CommonConsts.MaxLength32.中共预备党员 10.共青团员）
        /// </summary>
        [Description("政治面貌 （ 0.群众 30.中共党员 CommonConsts.MaxLength32.中共预备党员 10.共青团员）")]
        public PoliticsType PoliticsType { get; set; }

        /// <summary>
        /// 是否加入公会 (false否/true是)
        /// </summary>
        [Description("是否加入公会 (false否/true是)")]
        public bool? IsJoin { get; set; }

        /// <summary>
        /// 加入公会时间
        /// </summary>
        [Description("加入公会时间")]
        public DateTime? JoinTime { get; set; }

        /// <summary>
        /// 学历（0= 文盲,10= 小学 ,CommonConsts.MaxLength32= 初中,30= 中专,40= 高中,CommonConsts.MaxLength64= 大专,60= 本科,70= 硕士,80= 博士）
        /// </summary>
        [Description("学历（0= 文盲,10= 小学 ,CommonConsts.MaxLength32= 初中,30= 中专,40= 高中,CommonConsts.MaxLength64= 大专,60= 本科,70= 硕士,80= 博士）")]
        public EducationType? Education { get; set; }

        /// <summary>
        /// 是否有重病史（false:无 true:有）
        /// </summary>
        [Description("是否有重病史（false:无 true:有）")]
        public bool HasBadMedicalHistory { get; set; }

        /// <summary>
        /// 是否特种人员(false不是/true是)
        /// </summary>
        [Description("是否特种人员(false不是/true是)")]
        public bool IsSpecial { get; set; }

        #endregion
    }
}
