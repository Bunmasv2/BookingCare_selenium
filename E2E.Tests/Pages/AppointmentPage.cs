using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

public class AppointmentPage
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public AppointmentPage(IWebDriver driver)
    {
        this.driver = driver;
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
    }

    public void SelectDropdownByText(string id, string text)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(8));

        var selectElement = wait.Until(d =>
        {
            var element = d.FindElement(By.Id(id));
            var select = new SelectElement(element);

            return select.Options.Any(o => o.Text.Trim() == text)
                ? element
                : null;
        });

        new SelectElement(selectElement).SelectByText(text);
    }

    public void SelectDepartment(string value)
        => SelectDropdownByText("department", value);

    public void SelectDoctor(string value)
        => SelectDropdownByText("doctor", value);

    public void SelectService(string value)
        => SelectDropdownByText("service", value);

    public void SelectTime(string value)
        => SelectDropdownByText("appointmentTime", value);

    public void SelectSymptoms(string value)
        => SelectDropdownByText("symptoms", value);

    public void SelectDate(string date)
    {
        var dateInput = driver.FindElement(By.Id("appointmentDate"));
        IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

        js.ExecuteScript(@"
        const input = arguments[0];
        const value = arguments[1];

        const nativeInputValueSetter =
            Object.getOwnPropertyDescriptor(
                window.HTMLInputElement.prototype,
                'value'
            ).set;

        nativeInputValueSetter.call(input, value);

        input.dispatchEvent(new Event('input', { bubbles: true }));
        input.dispatchEvent(new Event('change', { bubbles: true }));
    ", dateInput, date);
    }

    private IWebElement SubmitButton =>
        wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("btn-submit-appointment")));

    public void Submit()
    {
        // cho React render xong trước khi submit
        Thread.Sleep(300);
        SubmitButton.Click();
    }
}
