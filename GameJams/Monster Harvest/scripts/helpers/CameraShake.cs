using System;
using Godot;

namespace BloodHarvest.scripts.helpers;

public partial class CameraShake : Node
{
    [Export]
    private Noise _noise;
    
    private Camera3D? _camera;

    private float _decay = .8f;
    private float _maxRoll = .001f;

    private float _trauma;
    private float _traumaPower = 4;
    private float _noiseY;
    private float _noiseX;
    
    public static CameraShake Instance { get; private set; }

    public override void _EnterTree()
    {
        base._EnterTree();

        Instance = this;
    }

    public void RegisterCamera(Camera3D camera)
    {
        _camera = camera;

        _noiseX = Random.Shared.Next();

        SetProcess(true);
    }

    public void UnregisterCamera()
    {
        _camera = null;
        SetProcess(false);
    }

    private bool IsCameraValid => _camera is not null && _camera.NativeInstance != IntPtr.Zero;

    public void AddTrauma(float amount)
    {
        _trauma = Mathf.Min(_trauma + amount, 1);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (_camera is null || _camera.NativeInstance == IntPtr.Zero) return;

        _trauma = Mathf.Max(_trauma - _decay * (float)delta, 0);

        if (_trauma > 0)
        {
            Shake();
        }
        else
        {
            _camera.Rotation = new Vector3(_camera.Rotation.X, _camera.Rotation.Y, 0);
        }
    }

    private void Shake()
    {
        if (_camera is null) return;
        
        var amount = Mathf.Pow(_trauma, _traumaPower);
        _noiseY++;
        
        var rotation = _maxRoll * amount * _noise.GetNoise2D(_noiseX, _noiseY);
        _camera.RotateZ(rotation);
    }
}