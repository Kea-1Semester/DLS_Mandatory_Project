using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClassLibrary
{
    public class LobbyMessage
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public required string Message { get; set; }
    }
}
