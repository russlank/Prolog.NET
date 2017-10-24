using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prolog;
using Prolog.Code;

namespace MyTests
{
    public static class Test01
    {

        static Test01()
        {

        }

        public static void Run()
        {
            //string programText = @"
            //    fatherN(russlan,[kinan,abdo,sami]).
            //    fatherN(russlan,[kinan1,abdo1,sami1]).
            //    appendto(X,[],X).
            //    appendto(Xs,[X|Ys],[X|Zs]):-appendto(Xs,Ys,Zs).
            //    ismemberof(X,[H|_]):-X==H.
            //    ismemberof(X,[H|L]):-dif(X,H),ismemberof(X,L).
            //    ismemberof(_,[]):-false.
            //    include(X,Y,Y):-ismemberof(X,Y).
            //    include(X,Y,[X|Y]):-not(member(X,Y)).
            //    father1(X,Y):-fatherN(X,Z),ismemberof(Y,Z).
            //    ";
            string programText = @"
                fact(atom).
                fatherN(russlan,[kinan,abdo,sami]).
                fatherN(russlan,[kinan1,abdo1,sami1]).
                appendto(X,[],X).
                appendto(Xs,[X|Ys],[X|Zs]):-appendto(Xs,Ys,Zs).
                ismemberof(X,[H|Anon1]):-X==H.
                ismemberof(X,[H|L]):-dif(X,H), ismemberof(X,L).
                ismemberof(Anon1,[]):-fail.
                include(X,Y,Y):-ismemberof(X,Y).
                include(X,Y,[X|Y]):-not(ismemberof(X,Y)).
                father1(X,Y):-fatherN(X,Z),ismemberof(Y,Z).
                ";

            Program program = new Program();
            
            //string[] programLines = programText.Split('\n');            
            //foreach (string line in programLines)
            //{
            //    Console.WriteLine($">> {line}");
            //    CodeSentence[] sentences = Parser.Parse(line);
            //    if (sentences != null)
            //    {
            //        foreach (var sentence in sentences) program.Add(sentence);
            //    }
            //    else Console.WriteLine($"ERROR!");

            //}

            Console.WriteLine($"PARSING PROGRAM:\n{programText}");
            CodeSentence[] sentences = Parser.Parse(programText);
            if (sentences != null)
            {
                foreach (var sentence in sentences) program.Add(sentence);

                program.IsOptimized = true;
            }
            else Console.WriteLine($"ERROR!");

            

            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();

            string queriesText = @"
                :-fact(atom)
                :-appendto([a,b,c],[d,e,f],X)
                :-father1(russlan,kinan)
                :-father1(russlan,hasan)
                :-father1(russlan,X)
                :-father1(X,Y)
                :-fatherN(russlan,X)
                :-father1(X,kinan)
                :-include(c,[a,b],X)
                ";


            string[] queryLines = queriesText.Split('\n');

            foreach(var queryLine in queryLines)
            {
                string trimmedQueryLine = queryLine.Trim();

                if (!string.IsNullOrWhiteSpace(trimmedQueryLine))
                {
                    Console.WriteLine($"\n\nRUN QUERY: '{trimmedQueryLine}'\n");
                    CodeSentence[] querySentences = Parser.Parse(trimmedQueryLine);
                    if (querySentences != null)
                    {
                        foreach (var querySentence in querySentences)
                        {
                            PrologMachine machine = PrologMachine.Create(program, new Query(querySentence));
                            ExecutionResults results = machine.RunToSuccess();

                            var result = machine.QueryResults;
                            if (result?.Variables?.Count() > 0)
                            {
                                foreach (PrologVariable variable in result.Variables)
                                {
                                    Console.WriteLine($"{results} => NAME: {variable.Name} -- FULL NAME: {variable.FullName} -- REGISTER: {variable.Register} -- CONTAINER: [{string.Join("|", variable.Container.Select(x => $"{x.Name}:{x.Text}:({string.Join("&", x.Register.Select(y => y.ToString()))})"))}] -- TEXT: {variable.Text}");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"{results} => HAS RETURNED NO VARIABLES");
                            }
                            Console.WriteLine("Press ENTER to continue...");
                            Console.ReadLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("ERROR: Failed to parse query...");
                        Console.ReadLine();
                    }

                }
            }
        }
    }

    class MyProgram
    {
        static void Main(string[] args)
        {
            Test01.Run();
            Console.WriteLine("FINISHED! Press ENTER to continue...");
            Console.ReadLine();
        }
    }
}

