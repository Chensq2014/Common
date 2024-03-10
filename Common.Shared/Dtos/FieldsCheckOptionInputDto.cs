using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Dtos
{
    public class FieldsCheckOptionInputDto<TPrimaryKey> //where TPrimaryKey : struct
    {
        public List<FieldCheckOptionDto<TPrimaryKey>> Fields { get; set; }
    }
}
