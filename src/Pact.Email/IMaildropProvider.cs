using System.IO;

namespace Pact.Email
{
    /// <summary>
    /// Optionally used by the EmailSender service to write email data to the file system 
    /// </summary>
    public interface IMaildropProvider
    {
        /// <summary>
        /// Prepare a stream for writing the email data
        /// </summary>
        /// <returns></returns>
        public StreamWriter GetStreamWriter();
    }
}