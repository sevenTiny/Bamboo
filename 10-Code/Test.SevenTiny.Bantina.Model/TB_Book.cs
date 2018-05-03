using SevenTiny.Bantina.Bankinate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.SevenTiny.Bantina.Model
{
    public class TB_Book
    {
        /// <summary>
        /// construction method
        /// </summary>
        public TB_Book() { }

        // PK（identity）  
        [Key]
        public Guid BookUid { get; set; } = Guid.NewGuid();
        // 
        [Column]
        public String Title { get; set; } = "";
        // 
        [Column]
        public String Title2 { get; set; } = "";
        // 
        [Column]
        public String Volume { get; set; } = "";
        // 
        [Column]
        public String Dynasty { get; set; } = "";
        // 
        [Column]
        public Int32 CategoryId { get; set; }

        // 
        [Column]
        public String Functionary { get; set; } = "";
        // 
        [Column]
        public String Publisher { get; set; } = "";
        // 
        [Column]
        public String Version { get; set; } = "";
        // 
        [Column]
        public String FromBF49 { get; set; } = "";
        // 
        [Column]
        public String FromAF49 { get; set; } = "";
        // 
        [Column]
        public String ImageUris { get; set; } = "";
        // 
        [Column]
        public String Notice { get; set; } = "";

        [Column]
        public DateTime CreateTime { get; set; }

        [Column]
        public int IsDelete { get; set; }
    }
}
