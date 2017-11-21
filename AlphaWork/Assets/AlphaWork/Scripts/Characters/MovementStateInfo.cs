using UnityEngine;
using System.Collections;

public class MovementStateInfo  {

    public void SetPosition(Vector3 pos)
    {
        m_Position = pos;
    }
    public Vector3 GetPosition3D()
    {
        return m_Position;
    }
    public void SetMoveDir(float dir)
    {
        if (Mathf.Abs(m_MoveDir - dir) > 0.001f)
        {
            m_MoveDir = dir;
        }
    }
    public float GetMoveDir()
    {
        return m_MoveDir;
    }

    public bool IsMoving
    {
        get { return m_IsMoving; }
        set
        {
            m_IsMoving = value;
        }
    }

    public Vector3 TargetPosition
    {
        get { return m_TargetPosition; }
        set { m_TargetPosition = value; }
    }


    Vector3 m_Position;
    float m_MoveDir;
    bool m_IsMoving;
    Vector3 m_TargetPosition;
}
