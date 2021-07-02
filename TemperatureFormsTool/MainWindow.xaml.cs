using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TemperatureFormsTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Button Submit = this.FindName("Submit") as Button;
            Button Copy = this.FindName("Copy") as Button;
            TextBox Address = this.FindName("Address") as TextBox;

            Submit.Click += (object sender,RoutedEventArgs args)=>{
                int count = Panel.Children.Count;
                for(int i = 0; i < count; i++)
                {
                    Panel.Children.RemoveAt(0);
                }
                Uri uri = new Uri($"https://docs.google.com/forms/d/{Address.Text}/viewform");
                System.Diagnostics.Debug.WriteLine(uri.AbsoluteUri);
                List<Input> res = Parse(uri);
                Web.Source = uri;
                foreach(Input i in res)
                {
                    System.Diagnostics.Debug.WriteLine(i.Name+":"+i.Id);
                    StackPanel pn = new StackPanel();
                    pn.Orientation = Orientation.Horizontal;
                    Label name = new Label();
                    name.Name = "name";
                    TextBlock id = new TextBlock();
                    id.Name = "id";
                    name.Content = i.Name;
                    id.Visibility = System.Windows.Visibility.Hidden;
                    id.Text = i.Id;

                    pn.Children.Add(name);
                    pn.Children.Add(id);
                    TextBox b = new TextBox();
                    b.Name = "value";
                    b.MinWidth = 200;
                    if (i.Name.Contains("名"))
                    {
                        b.Text = "%name";
                    }
                    else if (i.Name.Contains("日"))
                    {
                        b.Text = "%date";
                    }
                    else if (i.Name.Contains("番"))
                    {
                        b.Text = "%number";
                    }
                    else if (i.Name.Contains("温"))
                    {
                        b.Text = "%temperature";
                    }
                    else if (i.Name.Contains("午"))
                    {
                        b.Text = "%time_convention";
                    }
                    pn.Children.Add(b);
                    Panel.Children.Add(pn);
                }
            } ;

            Copy.Click += (object sender, RoutedEventArgs args) =>
            {
                InputMapping mapping = new InputMapping();
                mapping.Mappings = new List<Mapping>();
                foreach(StackPanel pn in Panel.Children.OfType<StackPanel>())
                {
                    mapping.FormId = Address.Text;
                    Label name = pn.Children.OfType<Label>().First();
                    TextBlock id = pn.Children.OfType<TextBlock>().First();
                    TextBox value = pn.Children.OfType<TextBox>().First();
                    Mapping mp = new Mapping();
                    mp.Name = name.Content.ToString();
                    mp.InputId = id.Text;
                    mp.Value = value.Text;
                    mapping.Mappings.Add(mp);
                }
                Clipboard.SetData(DataFormats.Text, (Object)JsonConvert.SerializeObject(mapping,Formatting.Indented));
            };
        }
        
        public List<Input> Parse(Uri uri)
        {
            HttpClient client = new HttpClient();
      
            HttpResponseMessage responce = client.GetAsync(uri.AbsoluteUri).Result;
            var parser = new HtmlParser();
            IHtmlDocument doc = parser.ParseDocument(responce.Content.ReadAsStringAsync().Result);
            Console.WriteLine(doc.GetElementsByClassName("freebirdFormviewerViewNumberedItemContainer").Length);
            List<IElement> list = doc.GetElementsByClassName("freebirdFormviewerViewNumberedItemContainer").SelectMany(x => x.Children).Where(x => x.HasAttribute("data-params")).ToList();
            List<Input> result = new List<Input>();
            foreach (IElement e in list)
            {
                String json = e.GetAttribute("data-params").Replace("%.@.", "[");
                var myDeserializedClass = JsonConvert.DeserializeObject<JArray>(json);
                foreach (Object o in myDeserializedClass.Where(x => x is JArray).Children().Where(x => x is JArray).Children().Where(x => x is JArray).Children().Where(x => x is JValue).Where(x => !String.IsNullOrEmpty(x.ToString()) && int.TryParse(x.ToString(), out int i)))
                {
                    string name = e.GetElementsByClassName("exportItemTitle").First().ChildNodes.Where(x => x.NodeName == "#text").First().Text();
                    string id = o.ToString();
                    result.Add(new Input(name,id));
                }
            }
            return result;
        }
    }
}
