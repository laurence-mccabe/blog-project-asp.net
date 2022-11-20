using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace BlogProj_12_10_22.Enums
{
    public enum ModerationType
    {
        [Description("Language Problem")]
        Language,
        [Description("Political Propaganda")]
        Political,
        [Description("Drugs Problem")]

        Drugs,
        [Description("Threatening Problem")]

        Threatening,
        [Description("Sexual Problem")]

        Sexual,
        [Description("Hate Problem")]

        Hate,
        [Description("Shaming Problem")]

        Shaming
    }
}
