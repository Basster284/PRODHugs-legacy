<script setup lang="ts">
import { onMounted } from 'vue'
import { useHugsStore } from '@/stores/hugs'
import { Skeleton } from '@/components/ui/skeleton'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import { Avatar, AvatarFallback } from '@/components/ui/avatar'
import RankBadge from '@/components/RankBadge.vue'

const hugsStore = useHugsStore()

onMounted(() => {
  hugsStore.fetchLeaderboard(50, 0)
})
</script>

<template>
  <div class="mx-auto max-w-3xl space-y-6">
    <div>
      <h1 class="text-2xl font-semibold tracking-tight">Рейтинг</h1>
      <p class="text-muted-foreground">Топ пользователей по количеству обнимашек</p>
    </div>

    <div v-if="hugsStore.loading" class="space-y-3">
      <Skeleton v-for="i in 10" :key="i" class="h-12 w-full" />
    </div>

    <div v-else-if="hugsStore.leaderboard.length === 0" class="py-12 text-center text-muted-foreground">
      Пока нет данных
    </div>

    <div v-else class="rounded-[10px] border border-[#75988e33]">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead class="w-12">#</TableHead>
            <TableHead>Пользователь</TableHead>
            <TableHead>Ранг</TableHead>
            <TableHead class="text-right">Всего</TableHead>
            <TableHead class="text-right">Отправлено</TableHead>
            <TableHead class="text-right">Получено</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          <TableRow
            v-for="(entry, index) in hugsStore.leaderboard"
            :key="entry.user_id"
            class="cursor-pointer hover:bg-[#002D20]"
            @click="$router.push(`/user/${entry.user_id}`)"
          >
            <TableCell class="font-medium tabular-nums" :class="index === 0 ? 'text-prod-yellow' : ''">
              {{ index + 1 }}
            </TableCell>
            <TableCell>
              <div class="flex items-center gap-2">
                <Avatar class="size-7">
                  <AvatarFallback class="text-[10px]">
                    {{ entry.username.slice(0, 2).toUpperCase() }}
                  </AvatarFallback>
                </Avatar>
                <span class="font-medium">{{ entry.username }}</span>
              </div>
            </TableCell>
            <TableCell>
              <RankBadge :rank="entry.rank" />
            </TableCell>
            <TableCell class="text-right font-bold tabular-nums" :class="index === 0 ? 'text-prod-yellow' : ''">
              {{ entry.total_hugs }}
            </TableCell>
            <TableCell class="text-right tabular-nums text-muted-foreground">
              {{ entry.hugs_given }}
            </TableCell>
            <TableCell class="text-right tabular-nums text-muted-foreground">
              {{ entry.hugs_received }}
            </TableCell>
          </TableRow>
        </TableBody>
      </Table>
    </div>
  </div>
</template>
