namespace Program;

public static class Program {

    static readonly List<string> keymap = [
        "__undefined__",
        
        "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",

        "A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
        "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T",
        "U", "V", "W", "X", "Y", "Z",

        "f1", "f2", "f3", "f4", "f5", "f6", "f7", "f8", "f9", "f10", "f11", "f12",

        "ESC",
        "TAB",
        "SPACE",
        "ENTER",
        "CAPSLOCK",
        "BACKSPACE",
        
        "MINUS",
        "EQUALS",
        "COMMA",
        "DOT",
        
        "OEM_1",
        "OEM_2",
        "OEM_3",
        "OEM_4",
        "OEM_5",
        "OEM_6",
        "OEM_7",
        "OEM_10",
        "ABNT_C",

        "L_SHIFT", "R_SHIFT",
        "L_CONTROL", "R_CONTROL",
        "L_SUPER", "R_SUPER",
        "L_MENU", "R_MENU",

        "APPS",

        "PRINT_SCREEN",
        "SCROLL_LOC",
        "PAUSE",
        
        "INSERT", "DELETE",
        "HOME", "END",

        "PG_UP", "PG_DOWN",

        "UP",
        "LEFT",
        "DOWN",
        "RIGHT"
    ];

    public static int Main(string[] args) {

        if (args.Length != 1) {
            if (args.Length < 1) Console.WriteLine("No argument!\nUsage: kbdmapper <file>");
            if (args.Length > 1) Console.WriteLine("Too many arguments!\nUsage: kbdmapper <file>");
            return -1;
        }

        var fileName = args[0];

        if (!File.Exists(fileName)) {
            Console.WriteLine($"\"{fileName}\" is not a file!");
            return -1;
        }

        var newFileName = Path.Combine(Path.GetDirectoryName(fileName) ?? "", Path.GetFileNameWithoutExtension(fileName) + ".kbd");
        var content = File.ReadAllLines(fileName);

        var buffer = new byte[1024];

        foreach (var i in content) {
            if (i.Length <= 8) continue;

            byte byte0 = byte.Parse(i[0..2], System.Globalization.NumberStyles.HexNumber);
            byte byte1 = byte.Parse(i[4..6], System.Globalization.NumberStyles.HexNumber);
            var keyword = i[8..];

            ushort index = byte1;
            if (byte0 != 0) index |= 1 << 8;
            if (byte0 == 0xe1) index |= 1 << 7;
            
            if (!keymap.Contains(keyword)) {
                Console.WriteLine($"\x1b[0;31mKeyword \"{keyword}\" not recognized in the key map!\x1b[0;0m");
                continue;
            }

            Console.WriteLine($"Mapping {keyword,-20} ({byte0:X2}{byte1:X2}) to {index:X3} ({index>>8:b2}_{index & 0xFF:b8})");

            if (buffer[index] != 0) {
                Console.WriteLine($"\x1b[0;31mOverlapping scancodes in index {index:X2}"
                + $" (overlapping {keymap[buffer[index]]} with {keyword})!\x1b[0;0m");
                return 1;
            }
            buffer[index] =(byte)keymap.IndexOf(keyword);
        }

        File.WriteAllBytes(newFileName, buffer);
        return 0;
    }
}
