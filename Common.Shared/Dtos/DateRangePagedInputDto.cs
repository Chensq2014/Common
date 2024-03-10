﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Common.Interfaces;

namespace Common.Dtos
{
    /// <summary>
    /// 带时间范围的分页过滤
    /// </summary>
    public abstract class DateRangePagedInputDto : PagedInputDto, IDateRange, IValidatableObject
    {
        /// <summary>
        ///     开始时间
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        ///     结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            StartDate ??= (StartDate ??= DateTime.Now).AddMonths(-12);
            EndDate ??= StartDate.Value.AddMonths(12);
            var period = (EndDate - StartDate).Value.TotalDays;
            if (period < 0 || period > 365)
            {
                errors.Add(new ValidationResult("时间区间有误,结束日期不能早于开始日期且做多可查12个月内数据"));
            }

            return errors;
        }
    }
}