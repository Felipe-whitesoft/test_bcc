using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyApp.Namespace {
    public class FormsModel : PageModel {
        public string Message { get; private set; } = "PageModel in C#";
        public void OnGet() {
            Message += $" Server time is {DateTime.Now}";
        }
    }
}
