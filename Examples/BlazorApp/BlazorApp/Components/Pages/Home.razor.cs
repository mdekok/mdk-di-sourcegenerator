using BusinessLogic;
using BusinessBaseLogic;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Components.Pages;

public partial class Home : ComponentBase
{
    // Services in BusinessLogic assembly
    [Inject] required public MyService MyService { get; set; }
    [Inject] required public IInterface1 Interface1 { get; set; }
    [Inject] required public MyGenericService<int> MyGenericService { get; set; }
    [Inject] required public IInterface2<string> Interface2string { get; set; }
    [Inject] required public IInterface2<int> Interface2int { get; set; }
    [Inject] required public IInterface3 Interface3 { get; set; }
    [Inject] required public IInterface4 Interface4 { get; set; }

    // Services in BaseBusinessLogic assembly
    [Inject] required public MyBaseService MyBaseService { get; set; }
    [Inject] required public IBaseInterface BaseInterface { get; set; }
}
