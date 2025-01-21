using TP;

namespace TestTP1;

[TestFixture]
public class ProgramTests
{
    [Test]
    public void ConvertirChiffreRomain_InputValide_RetourneValeurAttendue()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Program.ConvertirChiffreRomain("I"), Is.EqualTo(1));
            Assert.That(Program.ConvertirChiffreRomain("V"), Is.EqualTo(5));
            Assert.That(Program.ConvertirChiffreRomain("X"), Is.EqualTo(10));
            Assert.That(Program.ConvertirChiffreRomain("L"), Is.EqualTo(50));
            Assert.That(Program.ConvertirChiffreRomain("C"), Is.EqualTo(100));
            Assert.That(Program.ConvertirChiffreRomain("D"), Is.EqualTo(500));
            Assert.That(Program.ConvertirChiffreRomain("M"), Is.EqualTo(1000));
            Assert.That(Program.ConvertirChiffreRomain("IV"), Is.EqualTo(4));
            Assert.That(Program.ConvertirChiffreRomain("IX"), Is.EqualTo(9));
            Assert.That(Program.ConvertirChiffreRomain("XL"), Is.EqualTo(40));
            Assert.That(Program.ConvertirChiffreRomain("XC"), Is.EqualTo(90));
            Assert.That(Program.ConvertirChiffreRomain("CD"), Is.EqualTo(400));
            Assert.That(Program.ConvertirChiffreRomain("CM"), Is.EqualTo(900));
            Assert.That(Program.ConvertirChiffreRomain("MCMXCIV"), Is.EqualTo(1994));
        });
    }

    [Test]
    public void ConvertirChiffreRomain_InputInvalide_LanceArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Program.ConvertirChiffreRomain(""));
        Assert.Throws<ArgumentException>(() => Program.ConvertirChiffreRomain("IIII"));
        Assert.Throws<ArgumentException>(() => Program.ConvertirChiffreRomain("VX"));
        Assert.Throws<ArgumentException>(() => Program.ConvertirChiffreRomain("IC"));
        Assert.Throws<ArgumentException>(() => Program.ConvertirChiffreRomain("IM"));
    }
}