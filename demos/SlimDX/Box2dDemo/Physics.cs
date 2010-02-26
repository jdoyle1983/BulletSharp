﻿using BulletSharp;
using DemoFramework;
using System;
using System.Drawing;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D9;

namespace Box2dDemo
{
    class Physics
    {
        ///create 25 (5x5) dynamic objects
        int ArraySizeX = 5, ArraySizeY = 5;

        ///scaling of the objects (0.1 = 20 centimeter boxes )
        public float Scaling = 1;

        CollisionDispatcher dispatcher;
        BroadphaseInterface broadphase;
        ConstraintSolver solver;
        AlignedObjectArray<CollisionShape> collisionShapes = new AlignedObjectArray<CollisionShape>();
        public DynamicsWorld world;

        public Physics()
        {
            CollisionConfiguration collisionConf;

            // collision configuration contains default setup for memory, collision setup
            collisionConf = new DefaultCollisionConfiguration();

            // Use the default collision dispatcher. For parallel processing you can use a diffent dispatcher.
            dispatcher = new CollisionDispatcher(collisionConf);

            VoronoiSimplexSolver simplex = new VoronoiSimplexSolver();
            MinkowskiPenetrationDepthSolver pdSolver = new MinkowskiPenetrationDepthSolver();

            Convex2dConvex2dAlgorithm.CreateFunc convexAlgo2d = new Convex2dConvex2dAlgorithm.CreateFunc(simplex, pdSolver);

            dispatcher.RegisterCollisionCreateFunc(BroadphaseNativeTypes.Convex2dShape, BroadphaseNativeTypes.Convex2dShape, convexAlgo2d);
            dispatcher.RegisterCollisionCreateFunc(BroadphaseNativeTypes.Box2dShape, BroadphaseNativeTypes.Convex2dShape, convexAlgo2d);
            dispatcher.RegisterCollisionCreateFunc(BroadphaseNativeTypes.Convex2dShape, BroadphaseNativeTypes.Box2dShape, convexAlgo2d);
            dispatcher.RegisterCollisionCreateFunc(BroadphaseNativeTypes.Box2dShape, BroadphaseNativeTypes.Box2dShape, new Box2dBox2dCollisionAlgorithm.CreateFunc());

            broadphase = new DbvtBroadphase();

            // the default constraint solver.
            solver = new SequentialImpulseConstraintSolver();

            world = new DiscreteDynamicsWorld(dispatcher, broadphase, solver, collisionConf);
            world.Gravity = new Vector3(0, -10, 0);

            // create a few basic rigid bodies
            CollisionShape groundShape = new BoxShape(150, 50, 150);
            collisionShapes.PushBack(groundShape);
            LocalCreateRigidBody(0, Matrix.Translation(0, -50, 0), groundShape);

            // create a few dynamic rigidbodies
            // Re-using the same collision is better for memory usage and performance
            float u = Scaling - 0.04f;

            Vector3[] points = {new Vector3(0, u, 0), new Vector3(-u, -u, 0), new Vector3(u,-u,0)};
            ConvexShape colShape = new Convex2dShape(new BoxShape(new Vector3(Scaling, Scaling, 0.04f)));
            colShape.Margin = 0.03f;
            collisionShapes.PushBack(colShape);
            float mass = 1.0f;
            Vector3 localInertia = colShape.CalculateLocalInertia(mass);

            Matrix startTransform;

            Vector3 x = new Vector3(-ArraySizeX, 8, -20);
		    Vector3 y;
		    Vector3 deltaX = new Vector3(Scaling, Scaling * 2, 0);
		    Vector3 deltaY = new Vector3(Scaling * 2, 0, 0);

            int i, j;
            for (i = 0; i < ArraySizeY; i++)
            {
                y = x;
                for (j = 0; j < ArraySizeX; j++)
                {
                    startTransform = Matrix.Translation(y - new Vector3(-5, 0, -15));

                    //using motionstate is recommended, it provides interpolation capabilities, and only synchronizes 'active' objects
                    DefaultMotionState myMotionState = new DefaultMotionState(startTransform);
                    RigidBodyConstructionInfo rbInfo = new RigidBodyConstructionInfo(mass, myMotionState, colShape, localInertia);
                    RigidBody body = new RigidBody(rbInfo);
                    //body.ActivationState = ActivationState.IslandSleeping;
                    body.LinearFactor = new Vector3(1, 1, 0);
                    body.AngularFactor = new Vector3(0, 0, 1);

                    world.AddRigidBody(body);
                    y += deltaY;
                }
                x += deltaX;
            }

            world.Broadphase.ResetPool(dispatcher);
            solver.Reset();
        }

        public void Update(float elapsedTime)
        {
            world.StepSimulation(elapsedTime);
        }

        public RigidBody LocalCreateRigidBody(float mass, Matrix startTransform, CollisionShape shape)
        {
            bool isDynamic = (mass != 0.0f);

            Vector3 localInertia = new Vector3(0, 0, 0);
            if (isDynamic)
                shape.CalculateLocalInertia(mass, out localInertia);

            DefaultMotionState myMotionState = new DefaultMotionState(startTransform);

            RigidBodyConstructionInfo rbInfo = new RigidBodyConstructionInfo(mass, myMotionState, shape, localInertia);
            RigidBody body = new RigidBody(rbInfo);

            world.AddRigidBody(body);

            return body;
        }
    }
}