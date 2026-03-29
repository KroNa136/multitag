using System.Threading.Tasks;
using Unity.Services.Multiplayer;
using UnityEngine;

public class MainMenu : Menu
{
    [SerializeField] private Menu _startServerMenu;
    [SerializeField] private Menu _connectMenu;

    public void StartServer()
    {
        _startServerMenu.Activate();
    }

    public void Connect()
    {
        _connectMenu.Activate();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
