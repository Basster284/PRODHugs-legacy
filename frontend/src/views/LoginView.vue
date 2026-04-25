<script setup lang="ts">
import { ref } from 'vue'
import { Heart } from 'lucide-vue-next'
import { useAuthStore } from '@/stores/auth'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from '@/components/ui/card'

const auth = useAuthStore()
const username = ref('')
const password = ref('')
const errorMsg = ref('')

async function handleLogin() {
  errorMsg.value = ''
  try {
    await auth.login(username.value, password.value)
  } catch (e: any) {
    errorMsg.value = e.response?.data?.message || 'Неверное имя пользователя или пароль'
  }
}
</script>

<template>
  <div class="flex min-h-screen items-center justify-center bg-background p-4">
    <Card class="w-full max-w-sm">
      <CardHeader class="text-center">
        <div class="mx-auto mb-2 flex size-10 items-center justify-center rounded-lg bg-primary text-primary-foreground">
          <Heart class="size-5" />
        </div>
        <CardTitle class="text-xl">Вход</CardTitle>
        <CardDescription>Войдите в свой аккаунт Hugs as a Service</CardDescription>
      </CardHeader>
      <CardContent>
        <form @submit.prevent="handleLogin" class="grid gap-4">
          <div class="grid gap-2">
            <Label for="username">Имя пользователя</Label>
            <Input
              id="username"
              v-model="username"
              type="text"
              placeholder="username"
              required
            />
          </div>
          <div class="grid gap-2">
            <Label for="password">Пароль</Label>
            <Input
              id="password"
              v-model="password"
              type="password"
              placeholder="********"
              required
            />
          </div>
          <p v-if="errorMsg" class="text-sm text-destructive text-center">
            {{ errorMsg }}
          </p>
          <Button type="submit" class="w-full" :disabled="auth.loading">
            {{ auth.loading ? 'Вход...' : 'Войти' }}
          </Button>
        </form>
      </CardContent>
      <CardFooter class="justify-center">
        <p class="text-sm text-muted-foreground">
          Нет аккаунта?
          <RouterLink to="/register" class="text-foreground underline underline-offset-4 hover:text-primary">
            Зарегистрироваться
          </RouterLink>
        </p>
      </CardFooter>
    </Card>
  </div>
</template>
