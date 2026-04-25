<script setup lang="ts">
import { useRoute } from 'vue-router'
import { computed } from 'vue'
import {
  LayoutDashboard,
  Users,
  Newspaper,
  Trophy,
  UserCircle,
  Heart,
} from 'lucide-vue-next'
import {
  Sidebar,
  SidebarContent,
  SidebarGroup,
  SidebarGroupContent,
  SidebarGroupLabel,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarRail,
} from '@/components/ui/sidebar'

const route = useRoute()

const items = [
  { title: 'Главная', url: '/dashboard', icon: LayoutDashboard },
  { title: 'Пользователи', url: '/users', icon: Users },
  { title: 'Лента', url: '/feed', icon: Newspaper },
  { title: 'Рейтинг', url: '/leaderboard', icon: Trophy },
  { title: 'Мой профиль', url: '/profile', icon: UserCircle },
]

const currentPath = computed(() => route.path)
</script>

<template>
  <Sidebar collapsible="icon">
    <SidebarHeader class="p-4">
      <div class="flex items-center gap-2 overflow-hidden">
        <div class="flex size-8 shrink-0 items-center justify-center rounded-lg bg-primary text-primary-foreground">
          <Heart class="size-4" />
        </div>
        <span class="truncate font-semibold">Hugs</span>
      </div>
    </SidebarHeader>
    <SidebarContent>
      <SidebarGroup>
        <SidebarGroupLabel>Навигация</SidebarGroupLabel>
        <SidebarGroupContent>
          <SidebarMenu>
            <SidebarMenuItem v-for="item in items" :key="item.title">
              <SidebarMenuButton
                as-child
                :is-active="currentPath === item.url || currentPath.startsWith(item.url + '/')"
              >
                <RouterLink :to="item.url">
                  <component :is="item.icon" />
                  <span>{{ item.title }}</span>
                </RouterLink>
              </SidebarMenuButton>
            </SidebarMenuItem>
          </SidebarMenu>
        </SidebarGroupContent>
      </SidebarGroup>
    </SidebarContent>
    <SidebarRail />
  </Sidebar>
</template>
