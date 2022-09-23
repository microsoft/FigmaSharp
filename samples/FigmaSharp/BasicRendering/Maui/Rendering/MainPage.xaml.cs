using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.Views;
using FigmaSharp.Services;
using FigmaSharp.Views.Maui;

namespace Rendering;

public class CustomButtonConverter : NodeConverter
{
    public override bool CanConvert(FigmaNode currentNode) =>
           currentNode.name.EndsWith("CustomButton", System.StringComparison.Ordinal);

    public override FigmaSharp.Views.IView ConvertToView(FigmaNode currentNode, ViewNode parent, ViewRenderService rendererService)
    {
        FigmaSharp.Views.Maui.Button button;
        if (currentNode is FigmaFrame figmaFrameEntity && !string.IsNullOrEmpty(figmaFrameEntity.transitionNodeID))
        {
            button = new TransitionButton()
            {
                TransitionNodeID = figmaFrameEntity.transitionNodeID,
                TransitionDuration = figmaFrameEntity.transitionDuration,
                TransitionEasing = figmaFrameEntity.transitionEasing,
            };
        }
        else
        {
            button = new FigmaSharp.Views.Maui.Button();
        }

        if (currentNode is IFigmaNodeContainer nodeContainer)
        {
            var text = nodeContainer.children
                .OfType<FigmaText>()
                .FirstOrDefault();

            if (text != null)
                button.Text = text.characters;

            var backgroundColor = nodeContainer.children.OfType<RectangleVector>().FirstOrDefault();
            if (backgroundColor != null && backgroundColor.HasFills)
                button.BackgroundColor = backgroundColor.fills[0].color;
        }

        button.TextColor = FigmaSharp.Color.White;
        return button;
    }

    public override bool ScanChildren(FigmaNode currentNode) => false;

    public override Type GetControlType(FigmaNode currentNode) => default;

    public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, ICodeRenderService rendererService) => string.Empty;
}

public class LoginTextFieldConverter : NodeConverter
{
    public override bool CanConvert(FigmaNode currentNode) =>
        currentNode.name.In("EmailTextField", "PasswordTextField");

    public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, ICodeRenderService rendererService) => string.Empty;

    public override FigmaSharp.Views.IView ConvertToView(FigmaNode currentNode, ViewNode parent, ViewRenderService rendererService)
    {
        var entry = new Entry();

        if (currentNode is IFigmaNodeContainer nodeContainer)
        {
            var text = nodeContainer.children
                .OfType<FigmaText>()
                .FirstOrDefault(s => s.name == "placeholderstring");
            if (text != null)
            {
                entry.Placeholder = text.characters;
            }
        }

        var view = new FigmaSharp.Views.Maui.View(entry);
        return view;
    }

    public override bool ScanChildren(FigmaNode currentNode) => false;

    public override Type GetControlType(FigmaNode currentNode) => default;
}

public partial class MainPage : RemoteContentPage
{
    const string LoginPage = "Log-In Page";
    const string ExhibitionPage = "Exhibition";
    const string HomePage = "Home";
    const string MenuPage = "Menu";

    public MainPage()
    {
        InitializeFigmaComponent();

        ContentView = new FigmaSharp.Views.Maui.View();

        LoadDocumentAsync("LuOj8jdGyCJkt4tzLbqSfh").ContinueWith(t =>
        {
            try
            {
                //we want render only the LoginPage node
                var node = RendererService.FindNodeByName(LoginPage);
                OnLoadLoginPage(node);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    void OnLoadLoginPage(FigmaSharp.Models.FigmaNode node)
    {
        //Background color
        SetValue(NavigationPage.BarBackgroundColorProperty, Colors.Black);
        //Title color
        SetValue(NavigationPage.BarTextColorProperty, Colors.White);
        BackgroundColor = Colors.Black;

        //we add some temporal converters process all the textfields into real views
        RenderViewWithConverters(node, new LoginTextFieldConverter(), new CustomButtonConverter());

        //adds some logic to the current rendered views
        var loginButton = RendererService.FindViewByName<FigmaSharp.Views.IButton>("LoginCustomButton");
        if (loginButton == null)
            return;

        loginButton.Clicked += (s, e) => {
            var emailTextField = FindNativeViewByName<Entry>("EmailTextField");
            var passwordTextField = FindNativeViewByName<Entry>("PasswordTextField");

            if (emailTextField.Text == "1234" && passwordTextField.Text == "1234")
            {
                //if (s is IViewTransitable figmaTransition)
                //    ProcessTransitionNodeID(figmaTransition.TransitionNodeID);
            }
            else
            {
                DisplayAlert("Credentials error", "You have entered a wrong user or password", "OK");
            }
        };

        void RenderViewWithConverters(FigmaSharp.Models.FigmaNode node, params NodeConverter[] converters)
        {
            if (converters != null && converters.Length > 0)
                AddConverter(converters);
            RenderByNode(node);
            if (converters != null && converters.Length > 0)
                RemoveConverter(converters);
        }
    }
}


