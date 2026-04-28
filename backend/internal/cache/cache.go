// Package cache provides a simple generic in-memory TTL cache.
package cache

import (
	"sync"
	"time"
)

// TTL is a concurrency-safe in-memory cache with a time-to-live per key.
type TTL[K comparable, V any] struct {
	mu      sync.RWMutex
	entries map[K]entry[V]
	ttl     time.Duration
}

type entry[V any] struct {
	value     V
	expiresAt time.Time
}

// New creates a TTL cache with the given default duration.
func New[K comparable, V any](ttl time.Duration) *TTL[K, V] {
	return &TTL[K, V]{
		entries: make(map[K]entry[V]),
		ttl:     ttl,
	}
}

// Get returns the cached value and true if it exists and has not expired.
func (c *TTL[K, V]) Get(key K) (V, bool) {
	c.mu.RLock()
	e, ok := c.entries[key]
	c.mu.RUnlock()

	if !ok || time.Now().After(e.expiresAt) {
		var zero V
		return zero, false
	}
	return e.value, true
}

// Set stores a value with the cache's default TTL.
func (c *TTL[K, V]) Set(key K, value V) {
	c.mu.Lock()
	c.entries[key] = entry[V]{value: value, expiresAt: time.Now().Add(c.ttl)}
	c.mu.Unlock()
}

// Invalidate removes a specific key.
func (c *TTL[K, V]) Invalidate(key K) {
	c.mu.Lock()
	delete(c.entries, key)
	c.mu.Unlock()
}

// InvalidateAll removes all entries.
func (c *TTL[K, V]) InvalidateAll() {
	c.mu.Lock()
	c.entries = make(map[K]entry[V])
	c.mu.Unlock()
}
