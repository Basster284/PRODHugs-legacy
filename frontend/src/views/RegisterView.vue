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
const passwordConfirm = ref('')
const errorMsg = ref('')

async function handleRegister() {
  errorMsg.value = ''
  if (password.value !== passwordConfirm.value) {
    errorMsg.value = 'Пароли не совпадают'
    return
  }
  if (password.value.length < 4) {
    errorMsg.value = 'Пароль должен быть не менее 4 символов'
    return
  }
  try {
    await auth.register(username.value, password.value)
  } catch (e: any) {
    errorMsg.value = e.response?.data?.message || 'Ошибка регистрации'
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
        <CardTitle class="text-xl">Регистрация</CardTitle>
        <CardDescription>Создайте аккаунт в Hugs as a Service</CardDescription>
      </CardHeader>
      <CardContent>
        <form @submit.prevent="handleRegister" class="grid gap-4">
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
          <div class="grid gap-2">
            <Label for="password-confirm">Подтверждение пароля</Label>
            <Input
              id="password-confirm"
              v-model="passwordConfirm"
              type="password"
              placeholder="********"
              required
            />
          </div>
          <p v-if="errorMsg" class="text-sm text-destructive text-center">
            {{ errorMsg }}
          </p>
          <Button type="submit" class="w-full" :disabled="auth.loading">
            {{ auth.loading ? 'Регистрация...' : 'Зарегистрироваться' }}
          </Button>
        </form>
      </CardContent>
      <CardFooter class="justify-center">
        <p class="text-sm text-muted-foreground">
          Уже есть аккаунт?
          <RouterLink to="/login" class="text-foreground underline underline-offset-4 hover:text-primary">
            Войти
          </RouterLink>
        </p>
      </CardFooter>
    </Card>
  </div>
</template>
