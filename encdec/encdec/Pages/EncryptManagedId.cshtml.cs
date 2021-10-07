using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using azure_manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace azure_manager.Pages
{
    public class EncryptManagedIdModel : PageModel
    {
        public EncryptManagedIdModel(ICrypto crypto)
        {
            _crypto = crypto;
        }

        private ICrypto _crypto;

        [BindProperty]
        public EncryptData EncryptData { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            EncryptData.Result = new ResponseData();

            if (!ModelState.IsValid)
            {
                EncryptData.Result.Error = "Invalid input";
                return Page();
            }

            try
            {
                EncryptData = await _crypto.EncryptAsync(EncryptData.PlainText, true);
            }
            catch (Exception ex)
            { 
                EncryptData.Result.Error = "Encryption Error: " + ex.Message;
                return Page();
            }

            return Page();
        }
    }
}
