using NUnit.Framework;

public class LevelManagerTests
{
   
    [Test]
    public void PuedeAcceder_NivelDesbloqueado()
    {
        LevelManager manager = new LevelManager();
        manager.nivelDesbloqueado = 1;

        bool resultado = manager.PuedeAccederNivel(1);

        Assert.IsTrue(resultado, "Debería poder acceder al nivel 1");
    }

   
    [Test]
    public void NoPuedeAcceder_NivelBloqueado()
    {
        LevelManager manager = new LevelManager();
        manager.nivelDesbloqueado = 1;

        bool resultado = manager.PuedeAccederNivel(2);

        Assert.IsFalse(resultado, "No debería poder acceder al nivel 2");
    }
}