
import { db } from "./db";
import { alerts, type Alert, type InsertAlert } from "@shared/schema";
import { eq } from "drizzle-orm";

export interface IStorage {
  getAlerts(): Promise<Alert[]>;
  createAlert(alert: InsertAlert): Promise<Alert>;
  deleteAlert(id: number): Promise<void>;
}

export class DatabaseStorage implements IStorage {
  async getAlerts(): Promise<Alert[]> {
    return await db.select().from(alerts);
  }

  async createAlert(insertAlert: InsertAlert): Promise<Alert> {
    const [alert] = await db.insert(alerts).values(insertAlert).returning();
    return alert;
  }

  async deleteAlert(id: number): Promise<void> {
    await db.delete(alerts).where(eq(alerts.id, id));
  }
}

export const storage = new DatabaseStorage();
