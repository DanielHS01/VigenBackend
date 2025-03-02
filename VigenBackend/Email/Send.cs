using MailKit.Security;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace Vigen_Repository.Email
{
    public class Send
    {
        public string enviar(string correo, string numero)
        {
            try
            {
                // Crear el mensaje de correo
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("vigenservice@gmail.com"));
                email.To.Add(MailboxAddress.Parse(correo));
                email.Subject = "Vigen Security Service";

                // Crear el cuerpo del correo en HTML
                email.Body = new TextPart(TextFormat.Html)
                {
                    Text = $@"
                        <html>
                            <body style='font-family: Arial, sans-serif; color: #333;'>
                                <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; max-width: 600px; margin: auto;'>
                                    <h2 style='color: #1a73e8;'>Vigen Security Service</h2>
                                    <p>Hola {correo},</p>
                                    <p>Hemos recibido una solicitud para obtener un código de seguridad en tu cuenta de Vigen.</p>
                                    <p style='font-size: 18px; font-weight: bold; color: #1a73e8;'>Este es tu código de seguridad:</p>
                                    <div style='font-size: 24px; font-weight: bold; color: #333; background-color: #e9ecef; padding: 10px; border-radius: 5px; text-align: center;'>
                                        {numero}
                                    </div>
                                    <p>Usa el código anterior para verificar tu identidad.</p>
                                    <p style='font-size: 12px; color: #777;'>Nota: Si no has registrado una cuenta con nuestro software, por favor ignora esta solicitud.</p>
                                    <hr style='border: none; border-top: 1px solid #ddd; margin-top: 20px;' />
                                    <p style='font-size: 12px; color: #777;'>Este mensaje fue enviado automáticamente por Vigen Security Service.</p>
                                </div>
                            </body>
                        </html>"
                };

                // Conectar y enviar el correo
                using (var smtp = new SmtpClient())
                {
                    smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    smtp.Authenticate("vigenservice@gmail.com", "hagzssosuisaozhy"); // Usar contraseña de aplicación
                    smtp.Send(email);
                    smtp.Disconnect(true);
                }

                return "success";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar correo: {ex.Message}");
                return ex.Message;
            }
        }
    }
}
