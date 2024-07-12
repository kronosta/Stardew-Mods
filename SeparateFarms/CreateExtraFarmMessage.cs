using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kronosta.SeparateFarms
{
    public class CreateExtraFarmMessage
    {
        public string Name { get; set; } = "";
        public string CloneFrom { get; set; } = "";
    }
}
