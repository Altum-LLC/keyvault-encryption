using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using azure_manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace azure_manager.Pages
{
    public class DecryptManagedIdModel : PageModel
    {
        public DecryptManagedIdModel(ICrypto crypto)
        {
            _crypto = crypto;
        }

        private ICrypto _crypto;

        [BindProperty]
        public DecryptData DecryptData { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            DecryptData.Result = new ResponseData();

            if (!ModelState.IsValid)
            {
                DecryptData.Result.Error = "Invalid input";
                return Page();
            }

            try
            {
                DecryptData = await _crypto.DecryptAsync(DecryptData.EncryptedText, true);
            }
            catch (Exception ex)
            {
                DecryptData.Result.Error = ex.Message;
                return Page();
            }

            return Page();
        }
    }
}
