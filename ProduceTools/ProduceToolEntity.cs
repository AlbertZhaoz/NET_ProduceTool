using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProduceTools
{
    public class ProduceToolEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public Repo Repo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Git Git { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public MsBuild MsBuild { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public WebClient WebClient { get; set; }
    }

    public class Repo
    {
        /// <summary>
        /// 
        /// </summary>
        public string DefaultPath { get; set; }
    }

    public class Git
    {
        /// <summary>
        /// 
        /// </summary>
        public string Path { get; set; }
    }

    public class MsBuild
    {
        /// <summary>
        /// 
        /// </summary>
        public string Path { get; set; }
    }

    public class WebClient
    {
        /// <summary>
        /// 
        /// </summary>
        public string WebHttp { get; set; }
    }
}
