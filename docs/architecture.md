# Coffee Codex — System Architecture

This document defines the core system model used by both the frontend and backend.

It describes:

- domain entities
- API contracts
- database schema
- enumerations

Both repositories share this architecture definition.

---

# Domain Model

The system revolves around several core entities.

## Recipe

Represents a coffee drink recipe.

A recipe contains:

- brew specifications
- ingredients
- preparation steps
- images
- tags
- author

---

## Author

Represents the creator of a recipe.

In the MVP this is primarily the admin user.

---

## RecipeIngredient

Represents an ingredient required for the drink.

Example:

Milk  
150 ml

---

## RecipeStep

Represents a step in the preparation process.

Example:

1. Whisk matcha with hot water
2. Add milk to glass
3. Pour espresso on top

---

## RecipeImage

Represents an image associated with a recipe.

Images are stored in blob storage.

---

## Tag

Tags describe recipe characteristics.

Examples:

- matcha
- citrus
- iced
- oat milk

---

# Brew Specifications

Brew specifications describe brewing parameters.

Example:

- coffee dose
- coffee yield
- milk volume
- cup size
- difficulty
- preparation time

Some drinks may not include coffee extraction (e.g. matcha drinks).

---

# Enumerations

## RecipeCategory

```

Classic
Modern
Citrus
Dessert
Iced

```

---

## DifficultyLevel

```

Beginner
Intermediate
Advanced

```

---

# API Contracts

These DTOs represent the API surface between frontend and backend.

---

## RecipeSummaryDto

Used in recipe listing.

Fields:

- id
- slug
- title
- category
- thumbnailUrl
- brewCount
- authorName
- difficulty

---

## RecipeDetailDto

Used in recipe page.

Fields:

- id
- title
- description
- category
- brewCount
- author
- brewSpecs
- ingredients
- steps
- images
- tags

---

## IngredientDto

Fields:

- name
- quantityValue
- unit

Example:

Milk  
150 ml

---

## StepDto

Fields:

- order
- instruction

---

## ImageDto

Fields:

- url
- caption
- order

---

## AuthorDto

Fields:

- id
- name
- avatarUrl

---

## BrewSpecsDto

Fields:

- coffeeDoseInGrams
- coffeeYieldInGrams
- milkInMl
- cupSizeInMl
- difficulty
- timeInMinutes

Coffee fields may be null for non-espresso drinks.

---

# Database Schema

Primary tables:

```

authors
recipes
recipe_brew_specs
recipe_ingredients
recipe_steps
recipe_images
tags
recipe_tags

```

---

## recipes

Core recipe entity.

Fields:

- id
- title
- description
- category
- slug
- author_id
- brew_count
- created_at
- updated_at

---

## recipe_brew_specs

Fields:

- recipe_id
- coffee_dose_in_grams
- coffee_yield_in_grams
- milk_in_ml
- cup_size_in_ml
- difficulty
- time_in_minutes

---

## recipe_ingredients

Fields:

- id
- recipe_id
- name
- quantity_value
- unit
- position

---

## recipe_steps

Fields:

- id
- recipe_id
- step_number
- instruction

---

## recipe_images

Fields:

- id
- recipe_id
- blob_url
- caption
- position
- created_at

---

## tags

Fields:

- id
- name

---

## recipe_tags

Many-to-many relation between recipes and tags.

Fields:

- recipe_id
- tag_id

---

# API Endpoints

Public APIs

```

GET /recipes
GET /recipes/{id}
GET /recipes/random
GET /recipes?search=
GET /recipes?category=
GET /recipes?tag=

```

Admin APIs

```

POST /recipes
PUT /recipes/{id}
DELETE /recipes/{id}
POST /recipes/{id}/images

```

---

# System Relationships

```

Author
└── Recipes

Recipe
├── BrewSpecs
├── Ingredients
├── Steps
├── Images
└── Tags

```

---

# Architectural Principles

The system follows these principles:

- simple relational data model
- clean API contracts
- frontend/backend separation
- minimal complexity for MVP

The architecture is intentionally lightweight but supports future growth.
