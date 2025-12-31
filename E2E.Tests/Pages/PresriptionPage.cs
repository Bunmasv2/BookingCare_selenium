using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

public class PrescriptionPage
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public PrescriptionPage(IWebDriver driver)
    {
        this.driver = driver;
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
    }

    private IWebElement Modal =>
        wait.Until(ExpectedConditions.ElementIsVisible(
            By.CssSelector(".modal.show")
        ));

    public void WaitForModalOpen()
    {
        Modal.ToString();
    }

    private IWebElement Diagnosis =>
        wait.Until(ExpectedConditions.ElementIsVisible(
            By.Id("diagnosis")
        ));

    public void EnterDiagnosis(string text)
    {
        Diagnosis.Clear();
        Diagnosis.SendKeys(text);
    }

    private IWebElement Treatment =>
        wait.Until(ExpectedConditions.ElementIsVisible(
            By.Id("treatment")
        ));

    public void EnterTreatment(string text)
    {
        Treatment.Clear();
        Treatment.SendKeys(text);
    }

    private IWebElement MedicineSearch =>
        wait.Until(ExpectedConditions.ElementIsVisible(
            By.Id("medicineSearch")
        ));

    private IWebElement MedicineSuggestion(string name) =>
        wait.Until(ExpectedConditions.ElementToBeClickable(
            By.XPath($"//div[contains(@class,'border-bottom')]//strong[text()='{name}']")
        ));

    public void SearchAndSelectMedicine(string medicineName)
    {
        MedicineSearch.Clear();
        MedicineSearch.SendKeys(medicineName);

        wait.Until(ExpectedConditions.ElementIsVisible(
            By.CssSelector(".position-absolute.bg-white")
        ));

        MedicineSuggestion(medicineName).Click();
    }

    private IWebElement Dosage =>
        wait.Until(ExpectedConditions.ElementIsVisible(By.Id("medicineDosage")));

    private IWebElement Frequency =>
        wait.Until(ExpectedConditions.ElementIsVisible(By.Id("medicineFrequency")));

    private IWebElement Duration =>
        wait.Until(ExpectedConditions.ElementIsVisible(By.Id("medicineDuration")));

    private IWebElement Usage =>
        wait.Until(ExpectedConditions.ElementIsVisible(By.Id("medicineUsage")));

    public void FillMedicineDetail(
        string dosage,
        string frequency,
        string duration,
        string usage
    )
    {
        Dosage.Clear();
        Dosage.SendKeys(dosage);

        Frequency.Clear();
        Frequency.SendKeys(frequency);

        Duration.Clear();
        Duration.SendKeys(duration);

        Usage.Clear();
        Usage.SendKeys(usage);
    }

    private IWebElement AddMedicineButton =>
        wait.Until(ExpectedConditions.ElementToBeClickable(
            By.XPath("//button[contains(.,'Thêm thuốc')]")
        ));

    public void AddMedicine()
    {
        AddMedicineButton.Click();
    }

    private IWebElement Notes =>
        wait.Until(ExpectedConditions.ElementIsVisible(
            By.Id("prescriptionNotes")
        ));

    public void EnterNotes(string text)
    {
        Notes.Clear();
        Notes.SendKeys(text);
    }

    private IWebElement SaveButton =>
        wait.Until(ExpectedConditions.ElementToBeClickable(
            By.XPath("//button[contains(.,'Lưu đơn thuốc')]")
        ));

    public void Submit()
    {
        Thread.Sleep(300);
        SaveButton.Click();
    }

    public string GetAlertMessage()
    {
        var alert = wait.Until(ExpectedConditions.AlertIsPresent());
        var text = alert.Text;
        alert.Accept();
        return text;
    }
}
