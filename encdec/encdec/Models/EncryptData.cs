using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace azure_manager.Models
{
    public class EncryptData
    {
        public ResponseData Result { get; set; }

        [Required, DataType(DataType.Text), StringLength(100)]
        public string PlainText { get; set; }

        public string EncryptedText { get; set; }

        public string Algorithm { get; set; }

        public string KeyId { get; set; }
    }
}
