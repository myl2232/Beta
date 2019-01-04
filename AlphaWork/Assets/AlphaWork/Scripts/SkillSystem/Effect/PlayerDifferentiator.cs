using UnityEngine;

namespace SkillSystem
{
  namespace PlayerDifferentiator
  {
    public interface IDifferentiator
    {
      bool Check(GameObject gameObject);
    }

    public class AllPlayers : IDifferentiator
    {
      bool IDifferentiator.Check(GameObject gameObject)
      {
        return true;
      }
    }

    public class LocalPlayerOnly : IDifferentiator
    {
      bool IDifferentiator.Check(GameObject gameObject)
      {
        return GlobalObj<ILocalPlayerDifferentiator>.Instance.IsLocalPlayer(gameObject);
      }
    }

    public class NonLocalPlayersOnly : IDifferentiator
    {
      bool IDifferentiator.Check(GameObject gameObject)
      {
        return !GlobalObj<ILocalPlayerDifferentiator>.Instance.IsLocalPlayer(gameObject);
      }
    }
  }
}
