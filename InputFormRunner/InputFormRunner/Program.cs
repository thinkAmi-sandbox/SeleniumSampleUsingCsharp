using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace InputFormRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var driver = new OpenQA.Selenium.Chrome.ChromeDriver())
            {
                driver.Navigate().GoToUrl("http://localhost:8000/site/register/");

                // <input type="text">
                EditTextField(driver);

                // <textarea>
                EditTextArea(driver);

                // <input type="text">
                EditDateField(driver);

                // <input type="radio">
                EditRadioField(driver);

                // <input type="checkbox">
                EditCheckboxField(driver);

                // 複数チェック可能な<input type="checkbox">
                EditMultiCheckboxField(driver);

                // <select>
                EditSelectField(driver);

                // <select multiple="multiple">
                EditMultiSelectField(driver);

                // スクリーンショットを撮って保存
                SaveScreenshot(driver);

                // フォームをsubmit
                SubmitForm(driver);


                // 勝手に終了しないようにする
                Console.WriteLine("何かキーを押すことで終了します");
                Console.ReadKey();
            }
        }


        /// <summary>
        /// input type="text"への入力・削除
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="elementId"></param>
        /// <param name="value"></param>
        private static void EditTextField(OpenQA.Selenium.IWebDriver driver)
        {
            var element = driver.FindElement(OpenQA.Selenium.By.Id("id_input_text"));

            // <input type="text">へデータを設定
            // なお、既にデータがあったり複数回の入力の場合、
            // データは追記されていく
            element.SendKeys("テキスト");
            element.SendKeys("txt");


            // <input type="text">のデータ削除
            element.Clear();

        }


        /// <summary>
        /// textareaへの入力・削除
        /// </summary>
        /// <param name="driver"></param>
        private static void EditTextArea(IWebDriver driver)
        {
            var element = driver.FindElement(OpenQA.Selenium.By.Id("id_text_area"));

            // <textarea>へデータを設定
            // なお、既にデータがあったり複数回の入力の場合、
            // データは追記されていく
            element.SendKeys("テキスト");
            element.SendKeys(OpenQA.Selenium.Keys.Enter);
            element.SendKeys("txt");


            // <input type="text">のデータ削除
            element.Clear();
        }


        /// <summary>
        /// input type="date"への入力・削除
        /// </summary>
        /// <param name="driver"></param>
        private static void EditDateField(IWebDriver driver)
        {
            var element = driver.FindElement(OpenQA.Selenium.By.Id("id_registration_date"));

            // <input type="date">へデータを設定
            // デフォルトでは本日日付が入っているWebアプリなので、
            // 年月日いずれも1つ前のデータを入力してみる
            var inputDate = DateTime.Now.AddYears(-1).AddMonths(-1).AddDays(-1);

            // ChromeDriverの場合、
            // 年の位置にカーソルがある状態で、
            // 年6桁・月2桁・日2桁という各桁数を持つ、
            // 前ゼロ詰め数字を入力すると、
            // 既存の値を上書きして設定される
            // ＊`2016/01/01`や`2016-01-01`ではうまく設定できず
            var year = string.Format("{0:D6}", inputDate.Year);
            var month = string.Format("{0:D2}", inputDate.Month);
            var day = string.Format("{0:D2}", inputDate.Day);
            element.SendKeys(year + month + day);


            // 後続の処理のために：
            // この時点ではカーソルが`日`の位置にあるため、
            // driver.FindElement()した時のカーソル位置(`年`)へ戻す
            // なお、Keysを連結して.SendKeys()に渡した場合、
            // 連結したキーをすべて含む動作をしてくれる
            element.SendKeys(Keys.ArrowLeft + Keys.ArrowLeft);


            // <input type="date">のデータ削除
            // ChromeDriverの場合、.Clear()ではエラーになる
            // invalid element state: Element must be user-editable in order to clear it.
            //element.Clear();

            // そのため、.SendKeys()でDeleteキーを使って削除する
            // ただ、Deleteキーを1回押すだけでは`年`だけが消えるので、カーソルキーを併用して
            // 1回目：年、2回目：月、3回目：日をそれぞれ消す
            for (int i = 0; i < 3; i++)
            {
                element.SendKeys(Keys.Delete);
                element.SendKeys(Keys.ArrowRight);
            }


            // データ削除したところに再入力する場合は、
            // 年の位置へカーソルを戻してから、再度入力する
            element.SendKeys(Keys.ArrowLeft + Keys.ArrowLeft);

            var reInputDate = DateTime.Now.AddYears(1).AddMonths(1).AddDays(1);
            var reYear = string.Format("{0:D6}", reInputDate.Year);
            var reMonth = string.Format("{0:D2}", reInputDate.Month);
            var reDay = string.Format("{0:D2}", reInputDate.Day);
            element.SendKeys(reYear + reMonth + reDay);
        }


        /// <summary>
        /// input type="radio"の選択
        /// </summary>
        /// <param name="driver"></param>
        private static void EditRadioField(IWebDriver driver)
        {
            var element = driver.FindElement(OpenQA.Selenium.By.Id("id_pushed_1"));
            element.Click();
        }


        /// <summary>
        /// input type="checkbox"の選択
        /// </summary>
        /// <param name="driver"></param>
        private static void EditCheckboxField(IWebDriver driver, string elementID = "id_checked")
        {
            var element = driver.FindElement(OpenQA.Selenium.By.Id(elementID));
            element.Click();
        }


        /// <summary>
        /// 複数input type="checkbox"の選択
        /// </summary>
        /// <param name="driver"></param>
        private static void EditMultiCheckboxField(IWebDriver driver)
        {
            EditCheckboxField(driver, "id_checked_multiple_1");
            EditCheckboxField(driver, "id_checked_multiple_2");
        }


        /// <summary>
        /// selectの選択
        /// </summary>
        /// <param name="driver"></param>
        private static void EditSelectField(IWebDriver driver)
        {
            // <select>の選択
            // OpenQA.Selenium.Support.UI(NuGet: Selenium.Supportで入る)の
            // SelectElement()を使って選択する
            var element = driver.FindElement(By.Id("id_selected"));
            var selectElement = new OpenQA.Selenium.Support.UI.SelectElement(element);

            // 選択方法は3種類
            // SelectByIndexは<option>のIndexに一致するもの
            selectElement.SelectByIndex(2);

            // SelectByValueは<option value="1">が一致するもの
            selectElement.SelectByValue("1");

            // SelectByTextは<option>の値が一致するもの
            selectElement.SelectByText("select3");
        }

        /// <summary>
        /// select-multipleの選択と解除
        /// </summary>
        /// <param name="driver"></param>
        private static void EditMultiSelectField(IWebDriver driver)
        {
            // <select multiple="multiple">の選択
            var element = driver.FindElement(By.Id("id_selected_multiple"));
            var selectElement = new SelectElement(element);

            // 全部を選択
            selectElement.SelectByValue("1");
            selectElement.SelectByValue("2");
            selectElement.SelectByValue("3");

            // 一部を解除
            // 選択と同様、Index, Value, Textの3種類あり
            selectElement.DeselectByValue("2");

            // もしくは、一括で解除
            selectElement.DeselectAll();

            // 再度選択しておく
            selectElement.SelectByValue("2");
        }


        /// <summary>
        /// スクリーンショットを撮って、デスクトップへ保存
        /// </summary>
        /// <param name="driver"></param>
        private static void SaveScreenshot(IWebDriver driver)
        {
            var s = ((OpenQA.Selenium.ITakesScreenshot)driver).GetScreenshot();
            s.SaveAsFile(System.IO.Path.Combine(
                System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), @"selenium.jpg"),
                System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        /// <summary>
        /// フォームをSubmit
        /// </summary>
        /// <param name="driver"></param>
        private static void SubmitForm(IWebDriver driver)
        {
            var element = driver.FindElement(By.Id("save"));
            element.Submit();
        }
    }
}
