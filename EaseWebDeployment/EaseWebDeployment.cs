using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EaseWebDeployment
{
    class EaseWebDeployment
    {
        string compilation_directory;
        string source_directory;

        string path;

        public EaseWebDeployment()
        {
            compilation_directory = "compiled";
            source_directory = "source";
            path = Directory.GetCurrentDirectory();
        }

        private void reset_compilation_directory()
        {
            try
            {
                Directory.Delete("compiled", true);
            }
            catch (DirectoryNotFoundException)
            {

            }
        }

        public void run()
        {
            reset_compilation_directory();

            Queue<string> directories = new Queue<string>();
            string source_path = path + @"\" + source_directory;
            if (Directory.Exists(source_path))
                directories.Enqueue(source_path);

            while (directories.Count > 0)
            {
                string directory = directories.Dequeue();

                foreach (string d in Directory.GetDirectories(directory))
                    directories.Enqueue(d);

                foreach (string f in Directory.GetFiles(directory))
                    process_file(f);
            }
        }

        private void process_file(string file_path)
        {
            string ext = Path.GetExtension(file_path);
            if (ext == ".html")
                process_html_file(file_path);
            else
                process_standard_file(file_path);
        }

        private void process_html_file(string file_path)
        {
            int i = file_path.IndexOf(Directory.GetCurrentDirectory());
            if (i == 0)
            {
                string relative_path = file_path.Substring(Directory.GetCurrentDirectory().Length);
                string new_path = "compiled" + relative_path;
                Directory.CreateDirectory(Path.GetDirectoryName(new_path));

                string content = File.ReadAllText(file_path);

                Lexer l = new Lexer();
                
                Parser p = new Parser();
                p.parse(l.lex(content));

                File.WriteAllText(new_path, content);
            }
        }

        private void process_standard_file(string file_path)
        {
            int i = file_path.IndexOf(Directory.GetCurrentDirectory());
            if (i == 0)
            {
                string relative_path = file_path.Substring(Directory.GetCurrentDirectory().Length);
                string new_path = "compiled" + relative_path;
                Directory.CreateDirectory(Path.GetDirectoryName(new_path));
                File.Copy(file_path, new_path);
            }
        }
    }
}
