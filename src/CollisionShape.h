#pragma once

#include "IDisposable.h"

using namespace System::Runtime::InteropServices;

namespace BulletSharp
{
	public ref class CollisionShape : BulletSharp::IDisposable
	{
	public:
		virtual event EventHandler^ OnDisposing;
		virtual event EventHandler^ OnDisposed;

	private:
		btCollisionShape* _collisionShape;

	internal:
		CollisionShape(btCollisionShape* collisionShape);
	public:
		!CollisionShape();
	protected:
		~CollisionShape();
	public:
		property bool IsDisposed
		{
			virtual bool get();
		}

	public:
		void CalculateLocalInertia(btScalar mass, [Out] Vector3% inertia);
		Vector3 CalculateLocalInertia(btScalar mass);
		void CalculateTemporalAabb(Matrix curTrans,
			Vector3 linvel,	Vector3 angvel, btScalar timeStep,
			[Out] Vector3% temporalAabbMin, [Out] Vector3% temporalAabbMax);
		void GetAabb(Matrix t, [Out] Vector3% aabbMin, [Out] Vector3% aabbMax);
		void GetBoundingSphere([Out] Vector3% center, [Out] btScalar% radius);
		btScalar GetContactBreakingThreshold(btScalar defaultContactThreshold);

		property btScalar AngularMotionDisc
		{
			btScalar get();
		}

		property bool IsCompound
		{
			bool get();
		}

		property bool IsConcave
		{
			bool get();
		}

		property bool IsConvex
		{
			bool get();
		}

		property bool IsConvex2d
		{
			bool get();
		}

		property bool IsInfinite
		{
			bool get();
		}

		property bool IsPolyhedral
		{
			bool get();
		}

		property bool IsSoftBody
		{
			bool get();
		}

		property Vector3 LocalScaling
		{
			Vector3 get();
			void set(Vector3 value);
		}

		property btScalar Margin
		{
			btScalar get();
			void set(btScalar value);
		}

		property String^ Name
		{
			String^ get();
		}

		property int ShapeType
		{
			int get();
		}

		property IntPtr UserPointer
		{
			IntPtr get();
			void set(IntPtr value);
		}

	internal:
		property btCollisionShape* UnmanagedPointer
		{
			btCollisionShape* get();
		}
	};
};