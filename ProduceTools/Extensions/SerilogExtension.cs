using Microsoft.Extensions.Options;
using ProduceTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Albert.Extensions
{
    public class SerilogExtension
    {
        private readonly IOptionsSnapshot<ProduceToolEntity> options;

        public SerilogExtension(IOptionsSnapshot<ProduceToolEntity> options)
        {
            this.options = options;
        }
    }
}
