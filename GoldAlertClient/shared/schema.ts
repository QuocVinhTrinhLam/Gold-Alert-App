
import { pgTable, text, serial, integer, boolean, timestamp } from "drizzle-orm/pg-core";
import { createInsertSchema } from "drizzle-zod";
import { z } from "zod";

export const alerts = pgTable("alerts", {
  id: serial("id").primaryKey(),
  targetPrice: integer("target_price").notNull(),
  condition: text("condition").notNull(), // "above" or "below"
  email: text("email").notNull(),
  createdAt: timestamp("created_at").defaultNow(),
});

export const insertAlertSchema = createInsertSchema(alerts).omit({ id: true, createdAt: true });

export type Alert = typeof alerts.$inferSelect;
export type InsertAlert = z.infer<typeof insertAlertSchema>;
