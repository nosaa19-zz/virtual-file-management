using System;
using System.Collections.Generic;
using System.Text;

namespace UsrControl.Domain
{
    class File
    {
        public String Id { get; set; }
        public String FolderId { get; set; }
        public String Name { get; set; }
        public String Extension { get; set; }
        public String Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
