using QRCoder;
using System.Drawing;

namespace VirusTracker.Business
{
    //https://github.com/codebude/QRCoder/

    public class QRCodeCreation
    {

        public Bitmap QRGenerate(string url)
        {

            PayloadGenerator.Url generator = new PayloadGenerator.Url(url);
            string payload = generator.ToString();

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            return qrCodeImage;


        }

    }
}
