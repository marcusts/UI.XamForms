
> *Xamarin.Forms Specific*

# A Light-Weight Framework for Modern, Animated Apps Using Responsive Tasks

&nbsp;
![](https://gitlab.com/marcusts1/nugetimages/-/raw/master/Modern_App_Demo_Master_FINAL.gif)

We have said -- *more than once* -- that a framework is a way of doing something very badly while pretending to save time.  Projects like **Prism** break more rules than a guest with Tourette's Syndrome at a tea party.  They claim to *"take care of the coding for you"*. But they do it by ***violating*** design principles and coding standards.  Those ideals aren't just *"polite rules for nice people"*.  They are what make an app safe, reliable, manageable, scalable, affordable, and user-friendly.  Design is ***everything***.

A real framework would ***leverage*** design principles to create a light-weight programming platform that ***promotes*** an exciting, involving user experience through an animated UI.  It would ***cure*** Xamarin's limitations.  It would aspire to greatness, but ***without*** trying to take over the world.

That's where our humble library comes in.  It isn't a black box.  It is ***tiny*** compare to so-called frameworks.  You can easily modify it or combine it with other tools.  Let's compare approaches to other coding paradigms:

| Topic                            | How We Handle It                 | How They Handle It             |
| :---                             | :---                             | :---                           |
| The Task Parallel Library &nbsp;| Head-on; ResponsiveTasks repairs broken TPL roots. We can even wait for void without hanging your app.&nbsp;|*"I see nothing! Nothing!"* Dozens of bad threads racing ***constantly*** &nbsp;|&nbsp;
| C# Fundamentals &nbsp;| 100% compliant. No redundancy. All interface contracts. &nbsp;| *"Magic"* file names illegally glue view models to views; if they don't match, the app crashes at run-time. **Extreme** redundancy *(2 million lines worth!)* Few interface contracts. &nbsp;|&nbsp;
| Navigation &nbsp;| Independent **App State Manager** identifies the app state using a set of handy string constants. **View Presenter** uses the app state to determine what view and view model should handle it based on ***run-time conditions***.  Flexible, open, and dynamic. &nbsp;| Tired, old "web" style menus and back-stack with rigid, hard-coded views and view models. Navigation occurs from a view model directly; this is illegal, since a view model cannot know what other views actually exist. &nbsp;|&nbsp;
| Page Management &nbsp;| Simple, single-page app with views that flow in and out like actors on a stage. Elegant, powerful, and light-weight coding. Fully animated. &nbsp;| Heavy-weight pages are constantly shuffled in and out with disruptive, ugly transitions. Virtually no animation. Like dancing with an elephant. &nbsp;|&nbsp;
| Views / Controls &nbsp;| Auto-generated views; can actually build an app with virtually no code in the view model except for simple attributes. &nbsp;| Horrid, redundant, impossible-to-maintain XAML; focus on static development.  Almost no actual code re-use and no way to control code bloat. &nbsp;|&nbsp;
| Editing / Input Strategy &nbsp;| Animated, floating placeholders for input fields. Flexible validation behaviors. Colorful cues below the field explain exactly what they must do for a proper input. All views validate as a group automatically. &nbsp;| Extremely tedious, verbose, old-style edit fields with clunky responses -- often through jolting modal dialogs. Extreme coding complexity. &nbsp;|&nbsp;

## We Even Produced a Complete Demo App 

For (a lot more detail on how we do this for you, see the [Modern App Demo on GitHub](https://github.com/marcusts/Com.MarcusTS.ModernAppDemo). *(This is not a NuGet library because it is not re-usable as a component. Just download the code and customize it as you see fit. It is ***100% runnable*** with a few demo views.)*

## Yes, You Can Still Use XAML 

*{Sigh}* 

The code in this project responds to any generic type. That can be a view created in XAML or in code:

```csharp
protected async Task ChangeContentView<InterfaceT, ClassT>(object viewModel)
    where ClassT : View, InterfaceT
    where InterfaceT : class
{
    { code omitted }

    await ChangeTheContent().AndReturnToCallingContext();

    // PRIVATE METHODS
    async Task ChangeTheContent()
    {
        var newView = _diContainer.RegisterAndResolveAsInterface<ClassT, InterfaceT>();
```

The **Modern App Demo** consumes this in the **Master View Presenter**:

```csharp
   public class MasterViewPresenter : 
       MasterViewPresenterBase, IMasterViewPresenter
   {
      protected override async Task RespondToViewModelChange(object newModule)
      {

         if (newModule is IDashboardViewModel)
         {
            await ChangeContentView<IDashboardTitledFlexViewHost, 
            DashboardTitledFlexViewHost>(newModule).AndReturnToCallingContext();
         }
         else if (newModule is ISettingsViewModel)
         {
            await ChangeContentView<ISettingsTitledFlexViewHost,
            SettingsTitledFlexViewHost>(newModule).AndReturnToCallingContext();
         }
         else if (newModule is IAccountsViewModel)
         {
            await ChangeContentView<IAccountsTitledFlexViewHost, 
            AccountsTitledFlexViewHost>(newModule).AndReturnToCallingContext();
         }
         else if (newModule is ILogInViewModel)
         {
            await ChangeContentView<ILogInTitledFlexViewHost, 
            LogInTitledFlexViewHost>(newModule).AndReturnToCallingContext();
         }
         else if (newModule is ICreateAccountViewModel)
         {
            await ChangeContentView<ICreateAccountTitledFlexViewHost, 
            CreateAccountTitledFlexViewHost>(newModule).AndReturnToCallingContext();
         }
         else if (newModule is ICreationSuccessViewModel)
         {
            await ChangeContentView<ICreationSuccessTitledFlexViewHost, 
            CreationSuccessTitledFlexViewHost>(newModule).AndReturnToCallingContext();
         }
      }
```

Simply replace any of these calls to **ChangeContentView** with a class and interface pointing to your XAML file.

## The Path from SharedForms to Here

You can still use our original SharedForms library for your apps.  This library leverages some of it, but in most ways, ***supercedes*** it due to the inclusion of the [Response Tasks Library](https://github.com/marcusts/Com.MarcusTS.ResponsiveTasks).  Once you have included a NuGet reference for this project, you might not need to directly reference **SharedForms** again.

## This Project Is Open Source; Enjoy Our Entire Public Suite 

### *Shared Utils (MAUI Ready!)*

[GutHub](https://github.com/marcusts/Com.MarcusTS.SharedUtils)

[NuGet](https://www.nuget.org/packages/Com.MarcusTS.SharedUtils)

### *The Smart DI Container (MAUI Ready!)*

[GutHub](https://github.com/marcusts/Com.MarcusTS.SmartDI)

[NuGet](https://www.nuget.org/packages/Com.MarcusTS.SmartDI)

### *Responsive Tasks (MAUI Ready!)*

[GutHub](https://github.com/marcusts/Com.MarcusTS.ResponsiveTasks)

[NuGet](https://www.nuget.org/packages/Com.MarcusTS.ResponsiveTasks)

### *PlatformIndependentShared (MAUI Ready!)*

[GutHub](https://github.com/marcusts/PlatformIndependentShared)

[NuGet](https://www.nuget.org/packages/Com.MarcusTS.PlatformIndependentShared)

### *UI.XamForms*

[GutHub](https://github.com/marcusts/UI.XamForms)

[NuGet](https://www.nuget.org/packages/Com.MarcusTS.UI.XamForms)

### *The Modern App Demo*

[GutHub](https://github.com/marcusts/Com.MarcusTS.ModernAppDemo)

&nbsp;
![](https://gitlab.com/marcusts1/nugetimages/-/raw/master/Modern_App_Demo_Master_FINAL.gif)
