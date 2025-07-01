export type Json =
  | string
  | number
  | boolean
  | null
  | { [key: string]: Json | undefined }
  | Json[]

export type Database = {
  graphql_public: {
    Tables: {
      [_ in never]: never
    }
    Views: {
      [_ in never]: never
    }
    Functions: {
      graphql: {
        Args: {
          operationName?: string
          query?: string
          variables?: Json
          extensions?: Json
        }
        Returns: Json
      }
    }
    Enums: {
      [_ in never]: never
    }
    CompositeTypes: {
      [_ in never]: never
    }
  }
  public: {
    Tables: {
      game_notes: {
        Row: {
          game_id: number
          id: string
          notes: string
          user_id: string
        }
        Insert: {
          game_id: number
          id?: string
          notes: string
          user_id: string
        }
        Update: {
          game_id?: number
          id?: string
          notes?: string
          user_id?: string
        }
        Relationships: [
          {
            foreignKeyName: "game_notes_game_id_fkey"
            columns: ["game_id"]
            isOneToOne: false
            referencedRelation: "games"
            referencedColumns: ["id"]
          },
          {
            foreignKeyName: "game_notes_user_id_fkey"
            columns: ["user_id"]
            isOneToOne: false
            referencedRelation: "profiles"
            referencedColumns: ["id"]
          },
        ]
      }
      games: {
        Row: {
          actions: Json
          board_state: Json
          created_by: string
          current_phase: number
          current_turn: number
          ended: string | null
          id: number
          logs: Json
          options: Json
          rounds: Json
          setup: Json
          started: string
          with_lost_fleet: boolean
        }
        Insert: {
          actions: Json
          board_state: Json
          created_by: string
          current_phase?: number
          current_turn: number
          ended?: string | null
          id?: number
          logs: Json
          options: Json
          rounds: Json
          setup: Json
          started?: string
          with_lost_fleet?: boolean
        }
        Update: {
          actions?: Json
          board_state?: Json
          created_by?: string
          current_phase?: number
          current_turn?: number
          ended?: string | null
          id?: number
          logs?: Json
          options?: Json
          rounds?: Json
          setup?: Json
          started?: string
          with_lost_fleet?: boolean
        }
        Relationships: [
          {
            foreignKeyName: "games_created_by_fkey"
            columns: ["created_by"]
            isOneToOne: false
            referencedRelation: "profiles"
            referencedColumns: ["id"]
          },
        ]
      }
      initial_game_states: {
        Row: {
          board: Json
          id: number
          players: Json
          setup_phase: Json
        }
        Insert: {
          board: Json
          id?: number
          players: Json
          setup_phase: Json
        }
        Update: {
          board?: Json
          id?: number
          players?: Json
          setup_phase?: Json
        }
        Relationships: [
          {
            foreignKeyName: "initial_game_states_id_fkey"
            columns: ["id"]
            isOneToOne: true
            referencedRelation: "games"
            referencedColumns: ["id"]
          },
        ]
      }
      profiles: {
        Row: {
          avatar_url: string | null
          birthdate: string | null
          first_name: string | null
          id: string
          last_name: string | null
          middle_name: string | null
          updated_at: string | null
          username: string
        }
        Insert: {
          avatar_url?: string | null
          birthdate?: string | null
          first_name?: string | null
          id: string
          last_name?: string | null
          middle_name?: string | null
          updated_at?: string | null
          username: string
        }
        Update: {
          avatar_url?: string | null
          birthdate?: string | null
          first_name?: string | null
          id?: string
          last_name?: string | null
          middle_name?: string | null
          updated_at?: string | null
          username?: string
        }
        Relationships: []
      }
    }
    Views: {
      [_ in never]: never
    }
    Functions: {
      [_ in never]: never
    }
    Enums: {
      activation_state:
        | "inactive"
        | "waiting_for_action"
        | "waiting_for_decision"
      faction:
        | "Terrans"
        | "Lantids"
        | "Taklons"
        | "Ambas"
        | "Gleens"
        | "Xenos"
        | "Ivits"
        | "HadschHallas"
        | "Bescods"
        | "Firaks"
        | "Geodens"
        | "BalTaks"
        | "Nevlas"
        | "Itars"
      notification_reason:
        | "game_started"
        | "your_turn"
        | "game_ended"
        | "game_deleted"
      notification_type: "generic" | "game"
    }
    CompositeTypes: {
      [_ in never]: never
    }
  }
}

type DefaultSchema = Database[Extract<keyof Database, "public">]

export type Tables<
  DefaultSchemaTableNameOrOptions extends
    | keyof (DefaultSchema["Tables"] & DefaultSchema["Views"])
    | { schema: keyof Database },
  TableName extends DefaultSchemaTableNameOrOptions extends {
    schema: keyof Database
  }
    ? keyof (Database[DefaultSchemaTableNameOrOptions["schema"]]["Tables"] &
        Database[DefaultSchemaTableNameOrOptions["schema"]]["Views"])
    : never = never,
> = DefaultSchemaTableNameOrOptions extends { schema: keyof Database }
  ? (Database[DefaultSchemaTableNameOrOptions["schema"]]["Tables"] &
      Database[DefaultSchemaTableNameOrOptions["schema"]]["Views"])[TableName] extends {
      Row: infer R
    }
    ? R
    : never
  : DefaultSchemaTableNameOrOptions extends keyof (DefaultSchema["Tables"] &
        DefaultSchema["Views"])
    ? (DefaultSchema["Tables"] &
        DefaultSchema["Views"])[DefaultSchemaTableNameOrOptions] extends {
        Row: infer R
      }
      ? R
      : never
    : never

export type TablesInsert<
  DefaultSchemaTableNameOrOptions extends
    | keyof DefaultSchema["Tables"]
    | { schema: keyof Database },
  TableName extends DefaultSchemaTableNameOrOptions extends {
    schema: keyof Database
  }
    ? keyof Database[DefaultSchemaTableNameOrOptions["schema"]]["Tables"]
    : never = never,
> = DefaultSchemaTableNameOrOptions extends { schema: keyof Database }
  ? Database[DefaultSchemaTableNameOrOptions["schema"]]["Tables"][TableName] extends {
      Insert: infer I
    }
    ? I
    : never
  : DefaultSchemaTableNameOrOptions extends keyof DefaultSchema["Tables"]
    ? DefaultSchema["Tables"][DefaultSchemaTableNameOrOptions] extends {
        Insert: infer I
      }
      ? I
      : never
    : never

export type TablesUpdate<
  DefaultSchemaTableNameOrOptions extends
    | keyof DefaultSchema["Tables"]
    | { schema: keyof Database },
  TableName extends DefaultSchemaTableNameOrOptions extends {
    schema: keyof Database
  }
    ? keyof Database[DefaultSchemaTableNameOrOptions["schema"]]["Tables"]
    : never = never,
> = DefaultSchemaTableNameOrOptions extends { schema: keyof Database }
  ? Database[DefaultSchemaTableNameOrOptions["schema"]]["Tables"][TableName] extends {
      Update: infer U
    }
    ? U
    : never
  : DefaultSchemaTableNameOrOptions extends keyof DefaultSchema["Tables"]
    ? DefaultSchema["Tables"][DefaultSchemaTableNameOrOptions] extends {
        Update: infer U
      }
      ? U
      : never
    : never

export type Enums<
  DefaultSchemaEnumNameOrOptions extends
    | keyof DefaultSchema["Enums"]
    | { schema: keyof Database },
  EnumName extends DefaultSchemaEnumNameOrOptions extends {
    schema: keyof Database
  }
    ? keyof Database[DefaultSchemaEnumNameOrOptions["schema"]]["Enums"]
    : never = never,
> = DefaultSchemaEnumNameOrOptions extends { schema: keyof Database }
  ? Database[DefaultSchemaEnumNameOrOptions["schema"]]["Enums"][EnumName]
  : DefaultSchemaEnumNameOrOptions extends keyof DefaultSchema["Enums"]
    ? DefaultSchema["Enums"][DefaultSchemaEnumNameOrOptions]
    : never

export type CompositeTypes<
  PublicCompositeTypeNameOrOptions extends
    | keyof DefaultSchema["CompositeTypes"]
    | { schema: keyof Database },
  CompositeTypeName extends PublicCompositeTypeNameOrOptions extends {
    schema: keyof Database
  }
    ? keyof Database[PublicCompositeTypeNameOrOptions["schema"]]["CompositeTypes"]
    : never = never,
> = PublicCompositeTypeNameOrOptions extends { schema: keyof Database }
  ? Database[PublicCompositeTypeNameOrOptions["schema"]]["CompositeTypes"][CompositeTypeName]
  : PublicCompositeTypeNameOrOptions extends keyof DefaultSchema["CompositeTypes"]
    ? DefaultSchema["CompositeTypes"][PublicCompositeTypeNameOrOptions]
    : never

export const Constants = {
  graphql_public: {
    Enums: {},
  },
  public: {
    Enums: {
      activation_state: [
        "inactive",
        "waiting_for_action",
        "waiting_for_decision",
      ],
      faction: [
        "Terrans",
        "Lantids",
        "Taklons",
        "Ambas",
        "Gleens",
        "Xenos",
        "Ivits",
        "HadschHallas",
        "Bescods",
        "Firaks",
        "Geodens",
        "BalTaks",
        "Nevlas",
        "Itars",
      ],
      notification_reason: [
        "game_started",
        "your_turn",
        "game_ended",
        "game_deleted",
      ],
      notification_type: ["generic", "game"],
    },
  },
} as const
