using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Dtos
{
    public class FieldCheckOptionInputDto<TPrimaryKey> //where TPrimaryKey : struct
    {
        public FieldCheckOptionDto<TPrimaryKey> Field{ get; set; }
    }
}
