using Albert.Interface;
using Microsoft.Extensions.Options;
using ProduceTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Albert.Extensions
{
    public class SerilogExtension: ISeriLog
    {
        private readonly IOptionsSnapshot<ProduceToolEntity> options;
        public string ExceptionlessClientDefaultStartUpKey { get; set; }
        public string SerilogFilePath { get; set; }

        public SerilogExtension(IOptionsSnapshot<ProduceToolEntity> options)
        {
            this.options = options;
            this.ExceptionlessClientDefaultStartUpKey = options.Value.SerilogConfig.ExceptionlessClientDefaultStartUpKey;
            this.SerilogFilePath = options.Value.SerilogConfig.SerilogFilePath;
        }

        public bool OpenExceptionlessClient()
        {
            if (string.IsNullOrEmpty(this.options.Value.SerilogConfig.ExceptionlessClientDefaultStartUpKey))
            {
                return false;
            }
            return true;
        }
    }
}
