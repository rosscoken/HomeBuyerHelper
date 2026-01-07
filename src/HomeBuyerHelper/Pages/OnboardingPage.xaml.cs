using HomeBuyerHelper.ViewModels;

namespace HomeBuyerHelper.Pages;

/// <summary>
/// Onboarding flow page.
/// </summary>
public partial class OnboardingPage : ContentPage
{
    private readonly OnboardingViewModel _viewModel;

    public OnboardingPage(OnboardingViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
        
        // Setup the Next/Complete button command binding based on current step
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(OnboardingViewModel.CurrentStep))
            {
                UpdateNextButtonCommand();
            }
        };
        
        UpdateNextButtonCommand();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Reset to step 1 when appearing
    }
    
    private void UpdateNextButtonCommand()
    {
        NextCompleteButton.Command = _viewModel.CurrentStep < _viewModel.TotalSteps 
            ? _viewModel.NextStepCommand 
            : _viewModel.CompleteOnboardingCommand;
    }
}
