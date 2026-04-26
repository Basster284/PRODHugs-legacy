import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: () => import('@/views/LoginView.vue'),
      meta: { guest: true },
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('@/views/RegisterView.vue'),
      meta: { guest: true },
    },
    {
      path: '/',
      redirect: '/dashboard',
    },
    {
      path: '/dashboard',
      name: 'dashboard',
      component: () => import('@/views/DashboardView.vue'),
      meta: { auth: true },
    },
    {
      path: '/users',
      name: 'users',
      component: () => import('@/views/UsersView.vue'),
      meta: { auth: true },
    },
    {
      path: '/user/:id',
      name: 'user-profile',
      component: () => import('@/views/UserProfileView.vue'),
      meta: { auth: true },
    },
    {
      path: '/leaderboard',
      name: 'leaderboard',
      component: () => import('@/views/LeaderboardView.vue'),
      meta: { auth: true },
    },
    {
      path: '/profile',
      redirect: '/dashboard',
    },
    {
      path: '/feed',
      name: 'feed',
      component: () => import('@/views/FeedView.vue'),
      meta: { auth: true },
    },
    {
      path: '/admin',
      name: 'admin',
      component: () => import('@/views/AdminView.vue'),
      meta: { auth: true, admin: true },
    },
  ],
})

router.beforeEach((to, _from, next) => {
  const token = localStorage.getItem('token')
  const userStr = localStorage.getItem('user')

  if (to.meta.auth && !token) {
    next('/login')
  } else if (to.meta.guest && token) {
    next('/dashboard')
  } else if (to.meta.admin) {
    const user = userStr ? JSON.parse(userStr) : null
    if (user?.role !== 'admin') {
      next('/dashboard')
    } else {
      next()
    }
  } else {
    next()
  }
})

export default router
