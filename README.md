# Jaggy Epub Translator

---

# Jaggy EPUB Translator

**The most gloriously unstable, painfully slow EPUB translator this side of a potato farm.**
Rips your `.epub` apart, throws it at Google Translate like a drunk intern, then Frankenstein-stitches it back together. Results may vary. Formatting may cry. You definitely will.

Perfect for light novels, web novels, or anything you're too impatient to wait for official translations of—just not too impatient to wait for *this* to finish.

---

## 💡 How It *Allegedly* Works

1. **Reads your EPUB** file like it's defusing a bomb.
2. **Scans the content** slowly and dramatically.
3. **Translates it** using Google Translate's API with the grace of a sloth doing ballet.
4. **Rebuilds the EPUB**, carefully reintroducing chaos.
5. **Saves it**, somewhere, hopefully where you told it to.

All this while taking enough time for you to learn Japanese instead.

---

## 🧾 Requirements

* Self-contained: No .NET install needed. Big fat exe. Works on machines with no clue what .NET is.
* Internet connection (unless you think Google Translate works on black magic)
* A bit of patience. Or a lot. Maybe make some tea.

---

## 🛠️ Installation

Download it from \[#linkhere] if you trust binaries made by strangers on the internet. Or build it yourself:

```bash
git clone https://github.com/YossefHawela/Jaggy-Epub-Translator
cd Jaggy_Epub_Translator
dotnet build
```

If it breaks, you get to keep both pieces.

---

## 🎮 Usage

```bash
./Jaggy_Epub_Translator.exe <filePath> <sourceLang> <targetLang> <outputPath>
```

### Arguments:

* `filePath`: Your `.epub` file – must exist, unlike your will to live after running this.
* `sourceLang`: The original language (like `ja` for Japanese, or `en` for English).
* `targetLang`: The language you *wish* you could read it in.
* `outputPath`: Folder or file path for the translated EPUB. If you leave it dumb, the program will guess something dumber.

### Example:

```bash
./Jaggy_Epub_Translator.exe "C:\Books\my-trash.epub" ja en "C:\Books\Output"
```

---

## 🐢 Performance

Yes, it's slow.
Yes, it could be optimized.
No, I won't do it.
**Speed: Somewhere between "watching paint dry" and "dial-up internet".**

If it crashes halfway, congrats, you're part of the beta test.

---

## 🪄 Features (kinda)

* Keeps formatting! (unless it doesn't)
* Translates all content! (eventually)
* Timestamped output so you know exactly how long this waste of time took
* Probably doesn’t delete your original file (probably)

---

## 🧠 FAQ

**Q: It’s not working!**
A: Shocking. Check your file path, language codes, and maybe your life choices.

**Q: It translated “hero” to “toaster”?**
A: Google Translate is the real villain here.

**Q: Is there a GUI?**
A: No. This tool is as user-friendly as Vim.

---

## 🧙 Contributing

Fork it. Break it more. Open a PR. Or just laugh and move on.
Repo: \[#linkhere]

---

## 🪦 Final Words

It's jaggy. It's slow. It's barely holding together.
But it gets the job done—eventually.
Just like you.

---

You’re welcome.
