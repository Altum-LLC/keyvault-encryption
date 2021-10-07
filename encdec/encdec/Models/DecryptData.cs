using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace azure_manager.Models
{
    public class DecryptData
    {
        public ResponseData Result { get; set; }

        [Required, DataType(DataType.Text), StringLength(1000)]
        public string EncryptedText { get; set; }

        public string DecryptedText { get; set; }
    }
}
