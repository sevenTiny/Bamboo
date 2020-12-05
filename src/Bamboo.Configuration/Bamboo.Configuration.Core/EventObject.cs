using System;
using System.Collections.Generic;
using System.Text;

namespace Bamboo.Configuration
{
    internal class EventObject
    {
        EventHandler handler;
        object sender;
        EventArgs args;

        public EventObject(EventHandler handler, object sender, EventArgs args)
        {
            this.handler = handler;
            this.sender = sender;
            this.args = args;
        }

        public void Execute()
        {
            handler(sender, args);
        }
    }
}
