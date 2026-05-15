public class LevelManager
{
    public int nivelDesbloqueado = 1;

    public bool PuedeAccederNivel(int nivel)
    {
        return nivel <= nivelDesbloqueado;
    }
}