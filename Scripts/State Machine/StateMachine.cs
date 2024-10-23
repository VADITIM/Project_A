using Godot;
using System;
using System.Collections.Generic;

public partial class StateMachine : Node
{

	[Export] public NodePath initialState;

	private Dictionary<string, State> states;
	private State currentState;

	public override void _Ready()
	{
		GD.Print("StateMachine _'Ready' executed");
		
		states = new Dictionary<string, State>();
		foreach (Node node in GetChildren())
		{
			if (node is State s)
			{
				states[node.Name] = s;
				s.stateMachine = this;
				GD.Print("State added: " + node.Name);  
				s.Exit();
			}
		}

		currentState = GetNode<State>(initialState);
		GD.Print("Initial state: " + initialState);
		currentState.Enter();
	}


	public override void _Process(double delta)
	{
		currentState.Update((float) delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		// currentState.PhysicsUpdate((float) delta);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		// currentState.HandleInput(@event);
	}

	public void Transition(string key)
	{
		if (!states.ContainsKey(key) || currentState == states[key])
			return;

		currentState.Exit();
		currentState = states[key];
		currentState.Enter();
	}
}
