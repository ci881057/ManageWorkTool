using System.Text;

class Program
{
    static readonly string BaseDir =
        @"C:\shutdown-log";

    static readonly string IdeaDir =
        Path.Combine(BaseDir, "ideas");

    static string Today =>
        DateTime.Now.ToString("yyyy-MM-dd");

    static string WorkFile =>
        Path.Combine(BaseDir, $"{Today}.md");

    static string IdeaFile =>
        Path.Combine(IdeaDir, $"{Today}.md");

    static string TodoFile =>
    Path.Combine(BaseDir, "todo.md");
    static string DoneTodoFile =>
    Path.Combine(BaseDir, "todo-done.md");

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Directory.CreateDirectory(BaseDir);
        Directory.CreateDirectory(IdeaDir);

        EnsureFile();
        ClearScreen();

        while (true)
        {
            Console.Write("> ");
            var cmd = Console.ReadLine()?.Trim();

            switch (cmd)
            {
                case "/q":
                    return;

                case "/1":
                    QuickLog();
                    break;

                case "/2":
                    FullLog();
                    break;

                case "/3":
                    ImproveLog();
                    break;

                case "/4":
                    IdeaLog();
                    break;

                case "/c":
                    ClearScreen();
                    break;
                case "/todo":
                    ShowTodo();
                    break;

                case "/done":
                    ViewDoneTodo();
                    break;
                default:

                    if (cmd.StartsWith("/v"))
                    {
                        var parts = cmd.Split(' ');

                        if (parts.Length == 1)
                            ViewSummary(Today);
                        else
                            ViewSummary(parts[1]);

                        break;
                    }

                    break;
            }
        }
    }

    // =====================
    // 畫面刷新
    // =====================

    static void ClearScreen()
    {
        Console.Clear();
        Console.WriteLine("\x1b[3J");
        var todoCount = GetTodoCount();
        Console.WriteLine(
  $"""
=== 記錄工具 {Today} ===

【工作記錄】
/1 快速記錄（多行，空行結束）
/2 工作整理（四項）
/3 觀察與改善（多行，空行結束）
/4 創作靈感（多行，空行結束）

【Todo】 ({todoCount})
/todo           查看 Todo
/done           查看 已完成Todo

【視圖】
/v              查看今天整理
/v yyyy-MM-dd   查看指定日期

【系統】
/c 清空畫面
/q 離開

""");
    }

    static int GetTodoCount()
    {
        if (!File.Exists(TodoFile))
            return 0;

        return File.ReadAllLines(TodoFile)
            .Count(x => !string.IsNullOrWhiteSpace(x) && !x.StartsWith("#"));
    }
    // =====================
    // 初始化檔案
    // =====================

    static void EnsureFile()
    {
        if (!File.Exists(WorkFile))
        {
            File.WriteAllText(
                WorkFile,
                $"# {Today} 工作記錄\n\n");
        }

        if (!File.Exists(IdeaFile))
        {
            File.WriteAllText(
                IdeaFile,
                $"# {Today} 創作靈感\n\n");
        }

        if (!File.Exists(TodoFile))
        {
            File.WriteAllText(
                TodoFile,
                "# Todo\n\n");
        }

        if (!File.Exists(DoneTodoFile))
        {
            File.WriteAllText(
                DoneTodoFile,
                "# Done Todo\n\n");
        }
    }

    // =====================
    // /1 快速記錄（多行）
    // =====================

    static void QuickLog()
    {
        var text = ReadMultiline("輸入內容（空行結束）：");

        if (string.IsNullOrWhiteSpace(text))
            return;

        File.AppendAllText(
            WorkFile,
            "\n快速記錄：\n" + text + "\n\n");

        Console.WriteLine("已儲存");
    }

    // =====================
    // /3 觀察與改善（多行）
    // =====================

    static void ImproveLog()
    {
        var text = ReadMultiline("輸入內容（空行結束）：");

        if (string.IsNullOrWhiteSpace(text))
            return;

        File.AppendAllText(
            WorkFile,
            "\n觀察與改善：\n" + text + "\n\n");

        Console.WriteLine("已儲存");
    }

    // =====================
    // /4 創作靈感（多行）
    // =====================

    static void IdeaLog()
    {
        var text = ReadMultiline("輸入內容（空行結束）：");

        if (string.IsNullOrWhiteSpace(text))
            return;

        File.AppendAllText(
            IdeaFile,
            "\n" + text + "\n\n");

        Console.WriteLine("已儲存");
    }

    // =====================
    // /2 工作整理（四項）
    // =====================

    static void FullLog()
    {
        Console.Write("今天進度：");
        var done = Console.ReadLine();

        Console.Write("下一步：");
        var next = Console.ReadLine();

        Console.Write("注意事項：");
        var note = Console.ReadLine();

        Console.Write("明天開始時：");
        var start = Console.ReadLine();

        var sb = new StringBuilder();
        sb.AppendLine();

        if (!string.IsNullOrWhiteSpace(done))
            sb.AppendLine($"今天進度：{done}");

        if (!string.IsNullOrWhiteSpace(next))
            sb.AppendLine($"下一步：{next}");

        sb.AppendLine();

        if (!string.IsNullOrWhiteSpace(note))
            sb.AppendLine($"注意事項：{note}");

        sb.AppendLine();

        if (!string.IsNullOrWhiteSpace(start))
            sb.AppendLine($"明天開始時：{start}");

        sb.AppendLine();

        File.AppendAllText(
            WorkFile,
            sb.ToString());

        Console.WriteLine("已儲存");
    }

    // =====================
    // 工具：多行輸入
    // =====================

    static string ReadMultiline(string prompt)
    {
        Console.WriteLine(prompt);

        var sb = new StringBuilder();

        while (true)
        {
            var line = Console.ReadLine();

            // 空行結束
            if (string.IsNullOrWhiteSpace(line))
                break;

            sb.AppendLine(line);
        }

        return sb.ToString().TrimEnd();
    }
    // =====================
    // 工具：視覺化整理
    // =====================
    static void ViewSummary(string date)
    {
        var workFile =
            Path.Combine(BaseDir, $"{date}.md");

        var ideaFile =
            Path.Combine(IdeaDir, $"{date}.md");

        Console.Clear();

        Console.WriteLine("══════════════════════════");
        Console.WriteLine($"【整理視圖】{date}");
        Console.WriteLine("══════════════════════════");

        if (!File.Exists(workFile) &&
            !File.Exists(ideaFile))
        {
            Console.WriteLine("\n沒有資料");
            WaitReturn();
            return;
        }

        var quick = new List<string>();
        var improve = new List<string>();
        var done = new List<string>();
        var next = new List<string>();
        var note = new List<string>();
        var start = new List<string>();
        var ideas = new List<string>();

        // ★ 新增這個工具函式
        static string RemovePrefix(string line, string prefix)
        {
            return line.Length >= prefix.Length
                ? line.Substring(prefix.Length)
                : "";
        }

        if (File.Exists(workFile))
        {
            var lines = File.ReadAllLines(workFile);

            string currentBlock = "";
            var paragraph = new StringBuilder();

            foreach (var line in lines)
            {
                if (line == "快速記錄：")
                {
                    FlushParagraph();
                    currentBlock = "quick";
                    continue;
                }

                if (line == "觀察與改善：")
                {
                    FlushParagraph();
                    currentBlock = "improve";
                    continue;
                }

                if (line.StartsWith("今天進度："))
                {
                    FlushParagraph();
                    done.Add(RemovePrefix(line, "今天進度："));
                    continue;
                }

                if (line.StartsWith("下一步："))
                {
                    FlushParagraph();
                    next.Add(RemovePrefix(line, "下一步："));
                    continue;
                }

                if (line.StartsWith("注意事項："))
                {
                    FlushParagraph();
                    note.Add(RemovePrefix(line, "注意事項："));
                    continue;
                }

                if (line.StartsWith("明天開始時："))
                {
                    FlushParagraph();
                    start.Add(RemovePrefix(line, "明天開始時："));
                    continue;
                }

                if (string.IsNullOrWhiteSpace(line))
                {
                    FlushParagraph();
                    continue;
                }

                paragraph.AppendLine(line);
            }

            FlushParagraph();

            void FlushParagraph()
            {
                if (paragraph.Length == 0)
                    return;

                var text = paragraph.ToString().TrimEnd();

                if (currentBlock == "quick")
                    quick.Add(text);

                else if (currentBlock == "improve")
                    improve.Add(text);

                paragraph.Clear();
            }
        }

        if (File.Exists(ideaFile))
        {
            ideas.AddRange(
                File.ReadAllLines(ideaFile)
                .Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        PrintBlock("快速記錄", quick);

        Console.WriteLine();
        Console.WriteLine("【工作整理】");
        Console.WriteLine("──────────────────────────");

        PrintSubBlock("今天進度", done);
        PrintSubBlock("下一步", next);
        PrintSubBlock("注意事項", note);
        PrintSubBlock("明天開始時", start);

        PrintBlock("觀察與改善", improve);

        PrintBlock("創作靈感", ideas);

        WaitReturn();
        return;
    }
    static void PrintBlock(string title, List<string> list)
    {
        Console.WriteLine();

        Console.WriteLine($"【{title}】 ({list.Count})");
        Console.WriteLine("──────────────────────────");

        if (list.Count == 0)
        {
            Console.WriteLine("(無)");
            return;
        }

        for (int i = 0; i < list.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {list[i]}");
        }
    }
    static void PrintSubBlock(string title, List<string> list)
    {
        Console.WriteLine();

        Console.WriteLine($"【{title}】 ({list.Count})");

        if (list.Count == 0)
        {
            Console.WriteLine("  (無)");
            return;
        }

        for (int i = 0; i < list.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {list[i]}");
        }
    }

    static void WaitReturn()
    {
        Console.WriteLine("\n按 Enter 返回...");
        Console.ReadLine();
        ClearScreen();
    }
    static void ShowTodo()
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine("══════════════════════════");
            Console.WriteLine("【Todo】");
            Console.WriteLine("══════════════════════════");

            var lines = File.ReadAllLines(TodoFile)
                .Where(x => !string.IsNullOrWhiteSpace(x) && !x.StartsWith("#"))
                .ToList();

            if (lines.Count == 0)
            {
                Console.WriteLine("\n(無)");
            }
            else
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {lines[i]}");
                }
            }

            Console.WriteLine("\n──────────────────────────");
            Console.WriteLine("1 完成    2 刪除    3 新增    q 返回");
            Console.Write("> ");

            var cmd = Console.ReadLine()?.Trim();

            if (cmd == "q")
            {
                ClearScreen();
                return;
            }

            if (cmd == "3")
            {
                Console.Write("新增 Todo：");
                var text = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(text))
                {
                    File.AppendAllText(TodoFile, text + "\n");
                }

                continue;
            }

            if (cmd == "1")
            {
                Console.Write("完成哪一項：");

                if (int.TryParse(Console.ReadLine(), out int index))
                {
                    if (index >= 1 && index <= lines.Count)
                    {
                        var completed = lines[index - 1];

                        // 加入 done 檔案（含日期）
                        File.AppendAllText(
                            DoneTodoFile,
                            $"[{DateTime.Now:yyyy-MM-dd}] {completed}\n");

                        lines.RemoveAt(index - 1);

                        File.WriteAllText(
                            TodoFile,
                            "# Todo\n\n" + string.Join("\n", lines) + "\n");
                    }
                }

                continue;
            }

            if (cmd == "2")
            {
                Console.Write("刪除哪一項：");

                if (int.TryParse(Console.ReadLine(), out int index))
                {
                    if (index >= 1 && index <= lines.Count)
                    {
                        lines.RemoveAt(index - 1);

                        File.WriteAllText(
                            TodoFile,
                            "# Todo\n\n" + string.Join("\n", lines) + "\n");
                    }
                }

                continue;
            }
        }
    }
    static void ViewDoneTodo()
    {
        Console.Clear();

        Console.WriteLine("══════════════════════════");
        Console.WriteLine("【已完成 Todo】");
        Console.WriteLine("══════════════════════════");

        var lines = File.ReadAllLines(DoneTodoFile)
            .Where(x => !string.IsNullOrWhiteSpace(x) && !x.StartsWith("#"))
            .ToList();

        if (lines.Count == 0)
        {
            Console.WriteLine("\n(無)");
        }
        else
        {
            for (int i = 0; i < lines.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {lines[i]}");
            }
        }

        WaitReturn();
    }
}
