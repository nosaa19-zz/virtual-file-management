using System;
using System.Collections.Generic;
using System.Text;

namespace UsrControl.Domain
{
    class Folder
    {
        public String Id { get; set; }
        public String UserId { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
